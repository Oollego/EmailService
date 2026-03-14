using EmailService.Domain.Entities;
using EmailService.Domain.ValueObjects;

namespace EmailService.Domain.Services
{
    public class EmailDomainService
    {
        // Например, логика подготовки письма для отправки
        public EmailLog CreateEmailLog(EmailAddress to, string subject, EmailBody body)
        {
            return new EmailLog(to.ToString(), subject, body.ToString());
        }

        // Можно добавить методы проверки Idempotency, генерации токена и т.д.
        //Любая бизнес-логика EmailService, которая не привязана к конкретному Handler
        //Служит "чистым" слоем между Entities и Application
        //DomainService → EmailDomainService (бизнес-логика на уровне домена, независимая от инфраструктуры)
    }
}
