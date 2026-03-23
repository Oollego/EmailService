using EmailService.Application.DTO;
using EmailService.Application.Interfaces;
using EmailService.Application.Models;
using Microsoft.AspNetCore.Mvc;
using RazorLight;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailPreviewController : ControllerBase
    {
        private readonly IRazorTemplateRenderer _renderer;
        private readonly ITemplateResolver _resolver;

        public EmailPreviewController(IRazorTemplateRenderer renderer, ITemplateResolver resolver)
        {
            _renderer = renderer;
            _resolver = resolver;
        }

        [HttpPost("preview")]
        public async Task<IActionResult> Preview([FromBody] EmailPreviewRequest request)
        {
            var template = _resolver.ResolveTemplate(request.Type, request.Language);

            var model = new TransactionalModel
            {
                UserName = request.Data.GetValueOrDefault("Name", "Test User"),
                Title = request.Subject,
                Message = request.Data.GetValueOrDefault("Message", "Test message"),
                ActionUrl = request.Data.GetValueOrDefault("Url"),
                ActionText = "Open",
                CreatedAt = DateTime.UtcNow
            };

            var html = await _renderer.RenderAsync(template, model);

            return Content(html, "text/html");

        }
    }
}


//        POST /api/email/preview

//{
//  "type": "Transactional",
//  "language": "en",
//  "subject": "Confirm your email",
//  "data": {
//    "Name": "Oleg",
//    "Message": "Please confirm your email",
//    "Url": "https://your-app.com/confirm"
//  }