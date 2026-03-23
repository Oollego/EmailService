using System.Transactions;
using EmailService.Application.Interfaces;
using EmailService.Application.Models;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Amqp.Framing;

namespace EmailService.Application.Handlers
{
    public class TransactionalEmailHandler : IEmailHandler
    {
        public EmailType Type => EmailType.Transactional;

        private readonly IRazorTemplateRenderer _renderer;
        private readonly ITemplateResolver _resolver;

        public TransactionalEmailHandler(IRazorTemplateRenderer renderer, ITemplateResolver resolver)
        {
            _renderer = renderer;
            _resolver = resolver;
        }

        public async Task<EmailLog> HandleAsync(EmailMessage message)
        {
            var template = _resolver.ResolveTemplate(message.Type, message.language);

            string body = await RenderTemplateAsync(template, message);

            var log = EmailDomainService.CreateEmailLogFromMessage(message, body);

            return log;
        }

        private async Task<string> RenderTemplateAsync(string template, EmailMessage message)
        {
            
            var model = new TransactionalModel()
            {
                UserName = message.Data["Name"],
                Title = message.Data["Title"],
                Message = message.Data["Message"],
                ActionUrl = message.Data["ActionUrl"],
                ActionText = message.Data["ActionText"],
                CreatedAt = DateTime.UtcNow,
            };

            //UserName = "Oleg",
            //Title = "Confirm your email",
            //Message = "Thank you for registering. Please confirm your email.",
            //ActionUrl = "https://your-app.com/confirm",
            //ActionText = "Confirm Email",
            //CreatedAt = DateTime.UtcNow

            // Рендерим шаблон
            return await _renderer.RenderAsync(template, model);
        }
    }
}
