namespace EmailService.Configuration
{
    public class ServiceBusOptions
    {
        /// <summary>
        /// Connection string к Azure Service Bus
        /// </summary>
        public string ConnectionString { get; set; } = null!;

        /// <summary>
        /// Имя queue или topic
        /// </summary>
        public string QueueName { get; set; } = null!;

        /// <summary>
        /// Максимальное количество сообщений в batch
        /// </summary>
        public int MaxConcurrentCalls { get; set; } = 5;

        /// <summary>
        /// Таймаут на обработку одного сообщения
        /// </summary>
        public int MessageProcessingTimeoutSeconds { get; set; } = 60;
    }
}
