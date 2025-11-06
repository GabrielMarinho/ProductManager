namespace ProductManager.Domain.Interfaces.Queue;

public interface IQueueEvents
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
    void Subscribe<T>(string queueName, Func<T, Task> handler);
}