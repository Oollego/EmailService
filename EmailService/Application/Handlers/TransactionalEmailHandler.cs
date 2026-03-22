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

        private readonly ITemplateRenderer _renderer;

        public TransactionalEmailHandler(ITemplateRenderer renderer)
        {
           _renderer = renderer;
        }

        public async Task<EmailLog> HandleAsync(EmailMessage message)
        {
            

            string body = RenderTemplate(message.Template, message.Data);

            var log = EmailDomainService.CreateEmailLogFromMessage(message, body);

            return log;
        }

        private async Task<string> RenderTemplateAsync(EmailMessage message)
        {
            
            var model = new WelcomeEmailModel
            {
                Name = message.Data["Name"] // пример
            };

            // Рендерим шаблон
            string body = await _renderer.RenderAsync("Transactional.cshtml", model);

            return EmailDomainService.CreateEmailLogFromMessage(message, body);
        }
    }
}
