using EmailService.Contracts.Enums;

namespace EmailService.Contracts.Message
{
    public class EmailMessage
    {
        public Guid Id { get; set; }

        public EmailType Type { get; set; }

        public string To { get; set; } = null!;

        public string Template { get; set; } = null!;

        public Dictionary<string, string> Data { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

    }
}
