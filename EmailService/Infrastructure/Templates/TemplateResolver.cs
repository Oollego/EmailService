using EmailService.Application.Interfaces;
using EmailService.Configuration;
using EmailService.Contracts.Enums;
using Microsoft.Extensions.Options;

namespace EmailService.Infrastructure.Templates
{
    public class TemplateResolver : ITemplateResolver
    {
        private const string DefaultLanguage = "en";
        private readonly string _basePath = "Infrastructure/Templates";
        private readonly TemplateOptions _options;

        public TemplateResolver(IOptions<TemplateOptions> options)
        {
            _options = options.Value;
        }

        public string ResolveTemplate(EmailType type, string? language)
        {
            var folder = type.ToString();

            string basePath = _options.TemplatePath ?? _basePath;

            var lang = string.IsNullOrWhiteSpace(language)
                ? DefaultLanguage
                : language.ToLower();

            var templatePath = Path.Combine(
                folder,
                $"{folder.ToLower()}_{lang}.cshtml"
            );

            if (File.Exists(Path.Combine(basePath, templatePath)))
                return templatePath;

            var fallbackPath = Path.Combine(
                folder,
                $"{folder.ToLower()}_{DefaultLanguage}.cshtml"
            );

            if (File.Exists(Path.Combine(basePath, fallbackPath)))
                return fallbackPath;

            var directory = Path.Combine(basePath, folder);

            if (Directory.Exists(directory))
            {
                var file = Directory.GetFiles(directory, "*.cshtml").FirstOrDefault();
                if (file != null)
                {
                    return Path.GetRelativePath(basePath, file);
                }
            }

            throw new FileNotFoundException($"No templates found for type {type}");
        }
    }
}

