using EmailService.Domain.Entities;
using EmailService.Domain.ValueObjects;

namespace EmailService.Domain.Services
{
    public class EmailDomainService
    {
        // Например, логика подготовки письма для отправки
        public EmailLog CreateEmailLog(
            EmailAddress to,
            string subject,
            EmailBody body,
            string? idempotencyKey = null)
        {
            var log = new EmailLog(to.Value, subject, body.Value);

            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                log.SetIdempotencyKey(idempotencyKey);
            }

            return log;
        }

        // Можно добавить методы проверки Idempotency, генерации токена и т.д.
        //Любая бизнес-логика EmailService, которая не привязана к конкретному Handler
        //Служит "чистым" слоем между Entities и Application
        //DomainService → EmailDomainService (бизнес-логика на уровне домена, независимая от инфраструктуры)
    }
}
