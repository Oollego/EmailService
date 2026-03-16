using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EmailService.Application.Handlers
{
    public class TransactionalEmailHandler : IEmailHandler
    {
        public EmailType Type => EmailType.Transactional;

        private readonly IEmailSendService _emailService;

        public TransactionalEmailHandler(IEmailSendService emailService)
        {
            _emailService = emailService;
        }

        public async Task<EmailLog> HandleAsync(EmailMessage message)
        {
            var log = new EmailLog(message.To, message.Subject, message.Template);

            try
            {
                await _emailService.SendAsync(log);
                log.MarkSent();
            }
            catch (Exception ex)
            {
                log.IncrementRetry(ex.Message);
            }

            return log;
        }

        public async Task SendExistingAsync(EmailLog log)
        {
           await _emailService.SendAsync(log);
        }
    }
}
