using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;

namespace EmailService.Application.Handlers
{
    public class VerificationEmailHandler : IEmailHandler
    {
        public EmailType Type => EmailType.EmailVerification;

        private readonly IEmailSendService _emailService;

        public VerificationEmailHandler(IEmailSendService emailService)
        {
            _emailService = emailService;
        }

        public async Task<EmailLog> HandleAsync(EmailMessage message)
        {
            var token = Guid.NewGuid();
            var link = $"https://site.com/verify?token={token}";
            var body = $"Verify your email: {link}";

            var log = new EmailLog(message.To, message.Subject, body, message.Id.ToString());

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
