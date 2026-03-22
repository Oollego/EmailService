using System.Text.Json;
using Azure.Messaging.ServiceBus;
using EmailService.Application.Interfaces;
using EmailService.Configuration;
using EmailService.Contracts.Message;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace EmailService.Worker
{
    public class EmailWorker : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailWorker> _logger;

        public EmailWorker(
           ServiceBusClient client,
           IServiceScopeFactory scopeFactory,
           IOptions<ServiceBusOptions> options,
           ILogger<EmailWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            _processor = client.CreateProcessor(options.Value.QueueName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = options.Value.MaxConcurrentCalls,
                AutoCompleteMessages = false
            });

            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("EmailWorker started");
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            using var scope = _scopeFactory.CreateScope();

            var processor = scope.ServiceProvider.GetRequiredService<IEmailProcessorService>();

            EmailMessage message;

            try
            {
                message = JsonSerializer.Deserialize<EmailMessage>(args.Message.Body)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid message format");
                await args.DeadLetterMessageAsync(args.Message);
                return;
            }

            try
            {
                await processor.ProcessAsync(message);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed for {Email}", message.To);

                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "ServiceBus error");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if( _processor != null )
            {
                await _processor.StopProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }

    }
}
