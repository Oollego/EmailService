using Azure.Messaging.ServiceBus;
using EmailService.Application.Interfaces;
using EmailService.Configuration;
using EmailService.Contracts.Message;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EmailService.Infrastructure.ServiceBus
{
    public class ServiceBusPublisher : IMessageBus
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(ServiceBusClient client, IOptions<ServiceBusOptions> options)
        {
            _sender = client.CreateSender(options.Value.QueueName);
        }

        public async Task<string> PublishAsync<T>(T message)
        {
            string json = JsonSerializer.Serialize(message);

            var serviceBusMessage = new ServiceBusMessage(json)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await _sender.SendMessageAsync(serviceBusMessage);

            return serviceBusMessage.MessageId;
        }
    }
}
