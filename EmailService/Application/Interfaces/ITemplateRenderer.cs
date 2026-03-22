namespace EmailService.Application.Interfaces
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync<T>(string templateName, T model);
    }
}
