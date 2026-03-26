
namespace EmailService.Application.Models.Verification
{
    public class VerificationModel : VerificationBaseModel
    {
        public string UserName { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string ActionUrl { get; set; } = null!;

        public string ButtonText { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
