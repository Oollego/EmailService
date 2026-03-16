namespace EmailService.Domain.Entities
{
    public class EmailLog
    {
        public Guid Id { get; private set; }

        public string To { get; private set; }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public bool IsSent { get; private set; }

        public int RetryCount { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTime? NextRetryAt { get; private set; }

        public string? IdempotencyKey { get; private set; }

        public EmailLog(string to, string subject, string body, string? idempotencyKey = null)
        {
            Id = Guid.NewGuid();
            To = to;
            Subject = subject;
            Body = body;
            CreatedAt = DateTime.UtcNow;
            IdempotencyKey = idempotencyKey;
        }

        public void SetIdempotencyKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Idempotency key cannot be null or empty.", nameof(key));
            }

            IdempotencyKey = key;
        }

        public void MarkSent()
        {
            IsSent = true;
            ErrorMessage = null;
            NextRetryAt = null;
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
