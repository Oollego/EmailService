namespace EmailService.Configuration
{
    public class SmtpOptions
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; } = true;
        public string DefaultFrom { get; set; } = null!;
    }
}
