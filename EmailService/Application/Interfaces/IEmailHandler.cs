using EmailService.Contracts.Enums;
using EmailService.Contracts.Message;
using EmailService.Domain.Entities;

namespace EmailService.Application.Interfaces
{
    public interface IEmailHandler
    {
        EmailType Type { get; }

        Task<EmailLog> HandleAsync(EmailMessage message);

    }
}
