using EmailService.Application.DTO;
using EmailService.Application.Interfaces;
using EmailService.Application.Models.Verification;
using EmailService.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using RazorLight;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/email/preview")]
    public class EmailPreviewController : ControllerBase
    {
        private readonly IRazorTemplateRenderer _renderer;
        private readonly ITemplateResolver _resolver;

        public EmailPreviewController(IRazorTemplateRenderer renderer, ITemplateResolver resolver)
        {
            _renderer = renderer;
            _resolver = resolver;
        }

        [HttpPost("verification")]
        public async Task<IActionResult> Preview([FromBody] EmailPreviewRequest request)
        {
            var template = _resolver.ResolveTemplate((EmailType)Enum.Parse(typeof(EmailType), request.Type), request.Language);

            var model = new VerificationModel
            {
                UserName = request.Data.GetValueOrDefault("Name", "Test User"),
                Title = request.Subject,
                Message = request.Data.GetValueOrDefault("Message", "Test message"),
                ActionUrl = request.Data.GetValueOrDefault("Url") ?? "https://unil.ink/",
                ButtonText = "Open",
                CreatedAt = DateTime.UtcNow
            };

            var html = await _renderer.RenderAsync(template, model);

            return Content(html, "text/html");

        }
    }
}


//        POST /api/email/preview

//{
//    "type": "Transactional",
//  "language": "en",
//  "subject": "Confirm your email",
//  "data": {
//        "Name": "Oleg",
//    "Message": "Please confirm your email",
//    "Url": "https://your-app.com/confirm"
//  }