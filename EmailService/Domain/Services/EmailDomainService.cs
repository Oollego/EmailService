using EmailService.Contracts.Message;
using EmailService.Domain.Entities;
using EmailService.Domain.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace EmailService.Domain.Services
{
    public static class EmailDomainService
    {
        // Например, логика подготовки письма для отправки
        public static EmailLog CreateEmailLog(
            EmailAddress to,
            string subject,
            EmailBody body,
            string idempotencyKey)
        {
            var log = new EmailLog(to.Value, subject, body.Value, idempotencyKey);

            return log;
        }

        public static EmailLog CreateEmailLogFromMessage(EmailMessage message, string? body = null)
        {
            EmailBody emailBody = new EmailBody(string.IsNullOrEmpty(body) ? message.Template : body);

            return CreateEmailLog(new EmailAddress(message.To), message.Subject, emailBody, message.Id.ToString());
        }

            // Можно добавить методы проверки Idempotency, генерации токена и т.д.
            //Любая бизнес-логика EmailService, которая не привязана к конкретному Handler
            //Служит "чистым" слоем между Entities и Application
            //DomainService → EmailDomainService (бизнес-логика на уровне домена, независимая от инфраструктуры)
    }
}
