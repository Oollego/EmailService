using EmailService.Contracts.Enums;

namespace EmailService.Application.DTO
{
    public class EmailPreviewRequest
    {
        public string Type { get; set; } = null!;

        public string Language { get; set; } = "en";

        public string Subject { get; set; } = null!;

        public Dictionary<string, string> Data { get; set; } = new();
    }
}
