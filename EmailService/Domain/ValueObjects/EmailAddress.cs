using System.Net.Mail;

namespace EmailService.Domain.ValueObjects
{
    public class EmailAddress
    {
        public string Value { get; private set; }

        public EmailAddress(string email) 
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email is re required");
            }

            if (!IsValid(email))
            {
                throw new ArgumentException("Email is not valid");
            }
        }

        private bool IsValid(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => Value; 
    }
}
