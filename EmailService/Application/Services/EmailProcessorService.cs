using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;
using EmailService.Domain.ValueObjects;

namespace EmailService.Application.Services
{
    public class EmailProcessorService : IEmailProcessorService
    {
        private readonly IEnumerable<IEmailHandler> _handlers;
        private readonly IEmailRepository _emailRepository;
        private readonly EmailDomainService _emailDomainService;

        public EmailProcessorService(
            IEnumerable<IEmailHandler> handlers,
            IEmailRepository emailRepository,
            EmailDomainService emailDomainService)
        {
            _handlers = handlers;
            _emailRepository = emailRepository;
            _emailDomainService = emailDomainService;
        }

        public async Task ProcessAsync(EmailMessage message)
        {
            string messageId = message.Id.ToString();

            if (!string.IsNullOrEmpty(messageId) && await _emailRepository.ExistsByIdempotencyKeyAsync(messageId))
            {
                return;
            }

            var handler = _handlers.First(x => x.Type == message.Type);

            EmailLog log;

            try
            {
                log = await handler.HandleAsync(message);
            }
            catch (Exception ex)
            {
                log = _emailDomainService.CreateEmailLog(
                    new EmailAddress(message.To),
                    message.Template,
                    new EmailBody(ex.Message)
                );

                log.IncrementRetry(ex.Message);
            }

            await _emailRepository.AddAsync(log);
        }

        public async Task ProcessRetryAsync(EmailLog log)
        {
            try
            {
                var handler = _handlers.First(x => x.Type == EmailType.Transactional);
                await handler.SendExistingAsync(log);
                await _emailRepository.MarkSentAsync(log.Id);
            }
            catch (Exception ex)
            {
                await _emailRepository.IncrementRetryAsync(log.Id, ex.Message);
            }
        }
    }
}
