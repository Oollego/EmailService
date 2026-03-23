using EmailService.Application.Interfaces;
using EmailService.Configuration;
using Microsoft.Extensions.Options;
using PreMailer.Net;
using RazorLight;

namespace EmailService.Infrastructure.Templates
{
    public class RazorTemplateRenderer : IRazorTemplateRenderer
    {
        private readonly RazorLightEngine _engine;
        private readonly ILogger<RazorTemplateRenderer> _logger;

        public RazorTemplateRenderer(IOptions<TemplateOptions> options, ILogger<RazorTemplateRenderer> logger)
        {
            _logger = logger;

            var root = options.Value.TemplatePath;
            var extension = options.Value.Extension;

            if (string.IsNullOrWhiteSpace(root))
                throw new ArgumentException("Template root path is not configured");

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Template extension is not configured");

            // Приводим к абсолютному пути
            var absolutePath = Path.IsPathRooted(root)
                ? root
                : Path.Combine(Directory.GetCurrentDirectory(), root);

            _logger.LogInformation("Razor templates path: {Path}", absolutePath);

            if (!Directory.Exists(absolutePath))
            {
                throw new DirectoryNotFoundException($"Templates folder not found: {absolutePath}");
            }

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(absolutePath, extension)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync<T>(string templateName, T model)
        {
            string html = await _engine.CompileRenderAsync(templateName, model);

            return PreMailer.Net.PreMailer.MoveCssInline(html).Html;
        }
    }
}
