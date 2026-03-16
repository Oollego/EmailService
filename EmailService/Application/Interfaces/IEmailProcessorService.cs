using EmailService.Contracts.Message;
using EmailService.Domain.Entities;

namespace EmailService.Application.Interfaces
{
    public interface IEmailProcessorService
    {
        Task ProcessAsync(EmailMessage message);
        Task ProcessRetryAsync(EmailLog log);
    }
}
