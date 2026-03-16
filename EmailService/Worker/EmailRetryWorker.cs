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

        public EmailRetryWorker(IServiceScopeFactory scopeFactory, IOptions<EmailOptions> options)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();
                var processor = scope.ServiceProvider.GetRequiredService<EmailProcessorService>();

                var emails = await repository.GetPendingEmailsAsync(_options.MaxRetryCount);

                foreach (var email in emails)
                {
                    await processor.ProcessRetryAsync(email);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
