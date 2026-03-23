using EmailService.Contracts.Enums;

namespace EmailService.Application.Interfaces
{
    public interface ITemplateResolver
    {
        string ResolveTemplate(EmailType type, string? language);
    }
}
