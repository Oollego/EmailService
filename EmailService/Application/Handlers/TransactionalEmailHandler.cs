using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EmailService.Application.Handlers
{
    public class TransactionalEmailHandler : IEmailHandler
    {
        public EmailType Type => EmailType.Transactional;

        private readonly IEmailSendService _emailService;
        private readonly IEmailRepository _emailRepository;

        public TransactionalEmailHandler(IEmailSendService emailService, IEmailRepository repository)
        {
            _emailService = emailService;
            _emailRepository = repository;
        }

        public async Task<EmailLog> HandleAsync(EmailMessage message)
        {
            

            ///Перенести шаблон в Handler или подумать еще как сделать
            string body = RenderTemplate(message.Template, message.Data);

            var log = EmailDomainService.CreateEmailLogFromMessage(message, body);

            return log;
        }

        private string RenderTemplate(string template, Dictionary<string, string> data)
        {
            // 🔹 Можно интегрировать любой TemplateRenderer
            foreach (var kvp in data)
                template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);

            return template;
        }

        public async Task SendExistingAsync(EmailLog log)
        {
           await _emailService.SendAsync(log);
        }
    }
}
