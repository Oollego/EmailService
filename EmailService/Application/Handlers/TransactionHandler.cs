using EmailService.Application.Interfaces;
using EmailService.Application.Models.Transaction;
using EmailService.Application.Models.Verification;
using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.Services;

namespace EmailService.Application.Handlers
{
    public class TransactionHandler : BaseEmailHandler<TransactionModel>
    {
        public override EmailType Type => EmailType.Transaction;

        public TransactionHandler(IRazorTemplateRenderer renderer, ITemplateResolver resolver)
            : base(renderer, resolver)
        {
        }

        protected override TransactionModel BuildModel(EmailMessage message)
        {
            var data = message.Data;

            return new TransactionModel
            {
                UserName = data.GetValueOrDefault("Name") ?? "",
                Title = data.GetValueOrDefault("Title") ?? message.Subject ?? "",
                Message = data.GetValueOrDefault("Message") ?? "",
                Code = data.GetValueOrDefault("Code") ?? "",
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
