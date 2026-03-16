using System.Net;
using System.Net.Mail;
using EmailService.Application.Interfaces;
using EmailService.Configuration;
using EmailService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace EmailService.Application.Services
{
    public class EmailSendService : IEmailSendService
    {
        private readonly SmtpOptions _options;

        public EmailSendService(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(EmailLog log, CancellationToken ct = default)
        {
            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.UserName, _options.Password),
                EnableSsl = _options.EnableSsl,
               
            };

            var message = new MailMessage
            {
                From = new MailAddress(_options.From),
                Subject = log.Subject,
                Body = log.Body,
                IsBodyHtml = true,
            };

            message.To.Add(log.To);

            await client.SendMailAsync(message, ct);
        }
    }
}
