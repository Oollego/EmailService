using EmailService.Application.Interfaces;
using RazorLight;
using PreMailer.Net;

namespace EmailService.Infrastructure.Templates
{
    public class RazorTemplateRenderer : IRazorTemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorTemplateRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject("Infrastructure/Templates/Templates")
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
