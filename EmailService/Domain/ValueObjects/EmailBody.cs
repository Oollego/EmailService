namespace EmailService.Domain.ValueObjects
{
    public class EmailBody
    {
        public string Value { get; private set; }

        public EmailBody(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Email body cannot be empty");
            }

            if (body.Length > 10000)
            {
                throw new ArgumentException("Email body is too long");

            }

            Value = body;
        }

        public override string ToString() => Value;
    }
}
