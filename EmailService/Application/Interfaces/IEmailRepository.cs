using EmailService.Domain.Entities;

namespace EmailService.Application.Interfaces
{
    public interface IEmailRepository
    {
        Task AddAsync(EmailLog log);

        Task MarkSentAsync(Guid id);

        Task IncrementRetryAsync(Guid id, string error);

        Task<bool> ExistsByIdempotencyKeyAsync(string key);

        Task<List<EmailLog>> GetPendingEmailAsync(int maxRetryCount);

        Task<EmailLog?> GetByIdAsync(Guid id);

        Task<List<EmailLog>> GetPendingEmailsAsync(int maxRetryCount);

    }
}
