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
            await _context.SaveChangesAsync();
        }

        public async Task MarkSentAsync(Guid id)
        {
            var email = await _context.EmailLogs.FindAsync(id);

            if(email == null)
            {
                return;
            }

            email.MarkSent();

            await _context.SaveChangesAsync();
        }

        public async Task IncrementRetryAsync(Guid id, string error)
        {
            var email = _context.EmailLogs.Find(id);

            if(email == null)
            {
                return;
            }

            email.IncrementRetry(error);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdempotencyKeyAsync(string key)
        {
            return await _context.EmailLogs.AnyAsync(x => x.Id.ToString() == key);
        }

        public async Task<List<EmailLog>> GetPendingEmailsAsync(int maxRetryCount)
        {
            return await _context.EmailLogs
                .Where(x => !x.IsSent && x.RetryCount < maxRetryCount)
                .OrderBy(x => x.CreatedAt)
                .Take(100)
                .ToListAsync();
        }

        public async Task<EmailLog?> GetByIdAsync(Guid id)
        {
            return await _context.EmailLogs.FindAsync(id);
        }
    }
}
