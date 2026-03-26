using EmailService.Application.Interfaces;
using EmailService.Application.Models.Verification;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;

namespace EmailService.Application.Handlers
{
    public class VerificationHandler : BaseEmailHandler<VerificationBaseModel>
    {
        public override EmailType Type => EmailType.Verification;

        public VerificationHandler( IRazorTemplateRenderer renderer, ITemplateResolver resolver)
            : base(renderer, resolver)
        {
        }

        protected override VerificationModel BuildModel(EmailMessage message)
        {
            var data = message.Data;

            return new VerificationModel
            {
                UserName = data.GetValueOrDefault("Name") ?? "",
                Title = data.GetValueOrDefault("Title") ?? message.Subject ?? "",
                Message = data.GetValueOrDefault("Message") ?? "",
                ActionUrl = data.GetValueOrDefault("ActionUrl")
                            ?? data.GetValueOrDefault("Url")
                            ?? "",
                ButtonText = data.GetValueOrDefault("ButtonText") ?? "Confirm",
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
