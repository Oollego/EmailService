using EmailService.Application.Interfaces;
using EmailService.Application.Services;
using EmailService.Configuration;
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
                var processor = scope.ServiceProvider.GetRequiredService<EmailProcessorService>();

                var emails = await repository.GetPendingEmailsAsync(_options.MaxRetryCount);

                _logger.LogInformation("RetryWorker: found {Count} emails", emails.Count);

                foreach (var email in emails)
                {
                    try
                    {
                        await processor.ProcessRetryAsync(email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Retry failed for email {Id}", email.Id);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.RetryIntervalSeconds), stoppingToken);
            }
        }
    }
}
