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
        private readonly string _templatesRoot;

        public RazorTemplateRenderer(IOptions<TemplateOptions> options, ILogger<RazorTemplateRenderer> logger)
        {
            _logger = logger;

            var root = options.Value.TemplatePath;
            var extension = options.Value.Extension;

            if (string.IsNullOrWhiteSpace(root))
                throw new ArgumentException("Template root path is not configured");

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Template extension is not configured");

            _templatesRoot = Path.IsPathRooted(root)
                ? root
                : Path.Combine(AppContext.BaseDirectory, root);

            _templatesRoot = Path.GetFullPath(_templatesRoot);

            _logger.LogInformation("Templates root: {Path}", _templatesRoot);

            if (!Directory.Exists(_templatesRoot))
            {
                throw new DirectoryNotFoundException(
                    $"Templates folder not found: {_templatesRoot}");
            }

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(_templatesRoot, extension)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync<T>(string templateName, T model)
        {
            try
            {
                _logger.LogInformation(
                    "Rendering template: {Template} with model: {Model}",
                    templateName,
                    typeof(T).Name);

                //"Transactional/Transactional_en.cshtml"
                var html = await _engine.CompileRenderAsync(templateName, model);

                var result = PreMailer.Net.PreMailer.MoveCssInline(html);

                // лог warnings от CSS
                if (result.Warnings.Any())
                {
                    foreach (var warning in result.Warnings)
                    {
                        _logger.LogWarning("CSS warning: {Warning}", warning);
                    }
                }

                return result.Html;
            }
            catch (TemplateNotFoundException ex)
            {
                _logger.LogError(ex, "Template not found: {Template}. Root: {Root}", templateName, _templatesRoot);

                throw;
            }
            catch (RazorLight.RazorLightException ex)
            {
                _logger.LogError(ex, "Razor rendering error for template: {Template}", templateName);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while rendering template: {Template}", templateName);

                throw;
            }
        }
    }
}
