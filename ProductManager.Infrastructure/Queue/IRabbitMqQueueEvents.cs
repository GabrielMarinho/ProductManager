using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Interfaces.Queue;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductManager.Infrastructure.Queue;

public class RabbitMqQueueEvents : IQueueEvents, IDisposable
{
    public const string ProductCreatedQueue = "productManager.product.created";

    private readonly ILogger<RabbitMqQueueEvents> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqQueueEvents(IConfiguration configuration, ILogger<RabbitMqQueueEvents> logger)
    {
        _logger = logger;

        var host = configuration["RabbitMq:Host"] ?? "localhost";
        var username = configuration["RabbitMq:Username"] ?? "guest";
        var password = configuration["RabbitMq:Password"] ?? "guest";
        var portStr = configuration["RabbitMq:Port"];
        int.TryParse(portStr, out var port);

        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
        };
        if (port > 0) factory.Port = port;

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        
        _channel.QueueDeclare(
            queue: ProductCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueName)) queueName = ProductCreatedQueue;

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var props = _channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2; // persistent

        _channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: props, body: body);
        _logger.LogInformation("[RabbitMQ] Published message to {Queue}", queueName);
        await Task.CompletedTask;
    }

    public void Subscribe<T>(string queueName, Func<T, Task> handler)
    {
        if (string.IsNullOrWhiteSpace(queueName)) queueName = ProductCreatedQueue;

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);
                if (message is not null)
                    await handler(message);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error processing message from {Queue}", queueName);
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("[RabbitMQ] Subscribed to {Queue}", queueName);
    }

    public void Dispose()
    {
        try
        {
            if (_channel.IsOpen) _channel.Close();
            if (_connection.IsOpen) _connection.Close();
        }
        catch
        {
            // ignore
        }
        _channel.Dispose();
        _connection.Dispose();
    }
}