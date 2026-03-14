using EmailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Infrastructure.Persistence
{
    public class EmailDbContext : DbContext
    {
        public DbSet<EmailLog> EmailLogs => Set<EmailLog>(); 

        public EmailDbContext(DbContextOptions<EmailDbContext> options) 
            : base(options) 
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<EmailLog>();

            builder.ToTable("EmailLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.To)
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(x => x.Body)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
           .IsRequired();

            builder.Property(x => x.IsSent)
                .IsRequired();

            builder.Property(x => x.RetryCount)
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.Property(x => x.NextRetryAt);

            builder.HasIndex(x => x.NextRetryAt);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.IsSent);
            builder.HasIndex(x => new { x.To, x.CreatedAt });

            base.OnModelCreating(modelBuilder);
        }

    }
}
