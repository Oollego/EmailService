namespace EmailService.Application.Interfaces
{
    public interface IRazorTemplateRenderer
    {
        Task<string> RenderAsync<T>(string templateName, T model);
    }
}
