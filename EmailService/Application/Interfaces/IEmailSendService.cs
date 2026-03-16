using EmailService.Domain.Entities;

namespace EmailService.Application.Interfaces
{
    public interface IEmailSendService
    {
        Task SendAsync(EmailLog log, CancellationToken ct = default);
    }
}
