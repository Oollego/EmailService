using EmailService.Application.Interfaces;
using EmailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace EmailService.Infrastructure.Persistence
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailDbContext _context;

        public EmailRepository(EmailDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailLog log)
        {
            _context.EmailLogs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (IsDuplicateKey(ex))
                {
                    return;
                }

                throw;
            }
        }

        private static bool IsDuplicateKey(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("UNIQUE") == true ||
                   ex.InnerException?.Message.Contains("duplicate") == true;
        }

        public async Task UpdateErrorMessageAsync(EmailLog log)
        {
            await _context.EmailLogs
                .Where(x => x.Id == log.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ErrorMessage, log.ErrorMessage)
                    .SetProperty(x => x.RetryCount, log.RetryCount)
                    .SetProperty(x => x.NextRetryAt, log.NextRetryAt));
        }

        public async Task MarkSentAsync(Guid id)
        {
            await _context.EmailLogs
               .Where(x => x.Id == id)
               .ExecuteUpdateAsync(setters => setters
                   .SetProperty(x => x.IsSent, true)
                   .SetProperty(x => x.ErrorMessage, (string?)null)
                   .SetProperty(x => x.NextRetryAt, (DateTime?)null));
        }

        public async Task IncrementRetryAsync(Guid id, string error)
        {
            var email = await _context.EmailLogs.FindAsync(id);

            if(email == null)
            {
                return;
            }

            email.IncrementRetry(error);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdempotencyKeyAsync(string key)
        {
            return await _context.EmailLogs.AnyAsync(x => x.IdempotencyKey == key);
        }

        public async Task<List<EmailLog>> GetPendingEmailsAsync(int maxRetryCount)
        {
            return await _context.EmailLogs
                    .Where(x =>
                        !x.IsSent &&
                        x.RetryCount < maxRetryCount &&
                        x.NextRetryAt <= DateTime.UtcNow)
                    .OrderBy(x => x.NextRetryAt)
                    .Take(100)
                    .ToListAsync();
        }

        public async Task<EmailLog?> GetByIdAsync(Guid id)
        {
            return await _context.EmailLogs.FindAsync(id);
        }
    }
}
