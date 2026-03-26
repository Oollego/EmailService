using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;

namespace EmailService.Application.Handlers
{
    public abstract class BaseEmailHandler<TModel> : IEmailHandler
    {
        protected readonly IRazorTemplateRenderer _renderer;
        protected readonly ITemplateResolver _resolver;

        protected BaseEmailHandler(IRazorTemplateRenderer renderer, ITemplateResolver resolver)
        {
            _renderer = renderer;
            _resolver = resolver;
        }

        public abstract EmailType Type { get; }

        protected abstract TModel BuildModel(EmailMessage message);

        public async Task<EmailLog> HandleAsync(EmailMessage message)
        {
            var template = _resolver.ResolveTemplate(Type, message.language);

            var model = BuildModel(message);

            var body = await _renderer.RenderAsync(template, model);

            return EmailDomainService.CreateEmailLogFromMessage(message, body);
        }
    }
}
