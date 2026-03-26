using EmailService.Contracts.Enums;

namespace EmailService.Application.DTO
{
    public class EmailBusDto
    {
        public string Type { get; set; } = null!;

        public string To { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string? language { get; set; }

        public Dictionary<string, string> Data { get; set; } = null!;
    }
}
