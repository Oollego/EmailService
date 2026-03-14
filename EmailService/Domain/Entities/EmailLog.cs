namespace EmailService.Domain.Entities
{
    public class EmailLog
    {
        public Guid Id { get; private set; }

        public string To { get; private set; } = null!;

        public string Subject { get; private set; } = null!;

        public string Body { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public bool IsSent { get; private set; }

        public int RetryCount { get; private set; }

        public string ErrorMessage { get; private set; } = null!;

        public DateTime? NextRetryAt { get; private set; }

        public EmailLog (string to, string subject, string body)
        {
            Id = Guid.NewGuid ();
            To = to;
            Subject = subject;
            Body = body;
            CreatedAt = DateTime.Now;
            IsSent = false;
            RetryCount = 0;
        }

        public void MarkSent()
        {
            IsSent = true;
        }

        public void IncrementRetry(string error)
        {
            RetryCount++;
            ErrorMessage = error;

            ScheduleNextRetry();
        }

        private void ScheduleNextRetry()
        {
            var delayMinutes = RetryCount switch
            {
                1 => 1,
                2 => 5,
                3 => 15,
                4 => 30,
                _ => 60
            };

            NextRetryAt = DateTime.UtcNow.AddMinutes(delayMinutes);
        }

    }
}
