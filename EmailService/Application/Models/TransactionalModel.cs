namespace EmailService.Application.Models
{
    public class TransactionalModel : EmailBaseModel
    {
        public string UserName { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string ActionUrl { get; set; } = null!;

        public string ActionText { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
