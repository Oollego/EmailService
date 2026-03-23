using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;
using EmailService.Domain.ValueObjects;
using EmailService.Worker;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Application.Services
{
    public class EmailProcessorService : IEmailProcessorService
    {
        private readonly IEnumerable<IEmailHandler> _handlers;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailSendService _emailService;
        private readonly ILogger<EmailProcessorService> _logger;

        public EmailProcessorService(
            IEnumerable<IEmailHandler> handlers,
            IEmailRepository emailRepository,
            IEmailSendService emailService,
            ILogger<EmailProcessorService> logger)
        {
            _handlers = handlers;
            _emailRepository = emailRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProcessAsync(EmailMessage message)
        {
            var handler = _handlers.First(x => x.Type == message.Type);

            EmailLog log;

            try
            {
                log = await handler.HandleAsync(message);
            }
            catch (Exception ex)
            {
                log = EmailDomainService.CreateEmailLogFromMessage(message);
                log.IncrementRetry(ex.Message);

                await _emailRepository.AddAsync(log);
                return;
            }

            try
            {
                await _emailRepository.AddAsync(log);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error while adding email to base {EmailId}", log.Id); 
                return;
            }

            try
            {
                await _emailService.SendAsync(log);

                log.MarkSent();
                await _emailRepository.MarkSentAsync(log.Id);
            }
            catch (Exception ex)
            {
                log.IncrementRetry(ex.Message);

                await _emailRepository.UpdateErrorMessageAsync(log);

                throw;
            }
        }

        public async Task ProcessRetryAsync(EmailLog log)
        {
            try
            {
                await _emailService.SendAsync(log);

                log.MarkSent();
                await _emailRepository.MarkSentAsync(log.Id);
            }
            catch (Exception ex)
            {
                log.IncrementRetry(ex.Message);
                await _emailRepository.IncrementRetryAsync(log.Id, ex.Message);
            }
        }
    }
}
