namespace EmailService.Application.Models.Transaction
{
    public class TransactionModel : TransactionBaseModel
    {
        public string UserName { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
