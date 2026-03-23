using EmailService.Application.Interfaces;
using EmailService.Application.Services;
using EmailService.Configuration;
using EmailService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace EmailService.Worker
{
    public class EmailRetryWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EmailOptions _options;
        private readonly ILogger<EmailRetryWorker> _logger;

        public EmailRetryWorker(IServiceScopeFactory scopeFactory, IOptions<EmailOptions> options, ILogger<EmailRetryWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();
                var processor = scope.ServiceProvider.GetRequiredService<IEmailProcessorService>();

                List<EmailLog> emails;

                try
                {
                    emails = await repository.GetPendingEmailsAsync(_options.MaxRetryCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RetryWorker: database error while fetching emails");

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                if (emails.Count == 0)
                {
                    _logger.LogInformation("RetryWorker: no emails to retry");
                }
                else
                {
                    _logger.LogInformation("RetryWorker: found {Count} emails", emails.Count);
                }

                foreach (var email in emails)
                {
                    try
                    {
                        await processor.ProcessRetryAsync(email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "RetryWorker: retry failed for email {Id}", email.Id);

                    }
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.RetryIntervalSeconds), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
