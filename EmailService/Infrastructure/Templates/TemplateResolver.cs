using EmailService.Application.Interfaces;
using EmailService.Contracts.Enums;

namespace EmailService.Infrastructure.Templates
{
    public class TemplateResolver : ITemplateResolver
    {
        private const string DefaultLanguage = "en";
        private readonly string _basePath = "Infrastructure/Templates/Templates";

        public string ResolveTemplate(EmailType type, string? language)
        {
            var folder = type.ToString();

            var lang = string.IsNullOrWhiteSpace(language)
                ? DefaultLanguage
                : language.ToLower();

            var templatePath = Path.Combine(
                folder,
                $"{folder.ToLower()}_{lang}.cshtml"
            );

            if (File.Exists(Path.Combine(_basePath, templatePath)))
                return templatePath;

            var fallbackPath = Path.Combine(
                folder,
                $"{folder.ToLower()}_{DefaultLanguage}.cshtml"
            );

            if (File.Exists(Path.Combine(_basePath, fallbackPath)))
                return fallbackPath;

            var directory = Path.Combine(_basePath, folder);

            if (Directory.Exists(directory))
            {
                var file = Directory.GetFiles(directory, "*.cshtml").FirstOrDefault();
                if (file != null)
                {
                    return Path.GetRelativePath(_basePath, file);
                }
            }

            throw new FileNotFoundException($"No templates found for type {type}");
        }
    }
}

