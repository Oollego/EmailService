using EmailService.Application.DTO;
using EmailService.Application.Interfaces;
using EmailService.Contracts.Message;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/email/bus")]
    public class EmailBusController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public EmailBusController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        ///summary>
        ///Send email message to bus
        ///</summary>
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] EmailBusDto message)
        {
            try
            {
                string messageId = await _messageBus.PublishAsync(message);

                return Ok(new
                {
                    message = "Email отправлен в очередь",
                    id = messageId,
                });
            }    
            catch (Exception)
            {
                return StatusCode(500, "Failed to send message");
            }
        }

//        {
//  "type": "Transactional",
//  "to": "oleg@merms.biz",
//  "subject": "Confirm your email",
//  "language": "en",
//  "data": {
//    "Name": "Oleg Kaaskoo",
//    "Title": "Confirm your email",
//    "Message": "Please confirm your email",
//    "ActionUrl": "https://your-app.com/confirm",
//    "ActionText": "Confirm Email"
//  }
//}
    }
}
