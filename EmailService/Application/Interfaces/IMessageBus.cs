namespace EmailService.Application.Interfaces
{
    public interface IMessageBus
    {
        Task<string> PublishAsync<T>(T message);
    }
}
