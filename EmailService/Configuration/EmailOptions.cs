namespace EmailService.Configuration
{
    public class EmailOptions
    {
        /// <summary>
        /// Минимальная задержка между отправкой писем (для Rate Limiter)
        /// </summary>
        public int SendDelayMilliseconds { get; set; } = 100;

        /// <summary>
        /// Максимальное количество попыток повторной отправки
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        public int RetryIntervalSeconds { get; set; } = 10;

        /// <summary>
        /// Default From address
        /// </summary>
        public string DefaultFrom { get; set; } = null!;

        /// <summary>
        /// Использовать тестовый режим (только логирование)
        /// </summary>
        public bool IsTestMode { get; set; } = false;
    }
}
