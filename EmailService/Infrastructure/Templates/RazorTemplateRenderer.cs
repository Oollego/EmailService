using EmailService.Application.Interfaces;
using RazorLight;

namespace EmailService.Infrastructure.Templates
{
    public class RazorTemplateRenderer : ITemplateRenderer
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
            return await _engine.CompileRenderAsync(templateName, model);
        }
    }
}
