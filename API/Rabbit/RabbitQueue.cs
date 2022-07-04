using Common;
using RabbitMQ.Client;

namespace API.Rabbit;

/// <summary>
/// Implementation of rabbit queue
/// </summary>
public class RabbitQueue : IRabbitQueue
{
    private readonly IConnection? connection;
    private readonly IModel? channel;

    private readonly ILogger<RabbitQueue> _logger;

    /// <inheritdoc/>
    public void SendMessage(Message message)
    {
        byte[]? body = message.GetRabbitBody();
        if(body == null || body.Length == 0)
            throw new Exception("Empty rabbit message");

        channel.BasicPublish("", message.GetQueueName(), null, body);
        _logger?.LogInformation("Message of type {type} has been sent through rabbit to queue {queueName}", message.GetMessageType(), message.GetQueueName());
    }

    /// <summary>
    /// Connects to the queue
    /// </summary>
    /// <param name="logger">Logger</param>
    public RabbitQueue(ILogger<RabbitQueue> logger)
    {
        _logger = logger;

        ConnectionFactory? factory = new() { HostName = GlobalSettings.rabbitHost };
        if (factory == null)
            throw new Exception("Could not create rabbit conection factory");

        connection = factory.CreateConnection();
        if (connection == null)
            throw new Exception("Could not create rabbit conection");

        channel = connection.CreateModel();
        if (channel == null)
            throw new Exception("Could not create rabbit channel");

        foreach(string declaredQueueName in Message.QueueNameForMessageType.Values)
            channel.QueueDeclare(declaredQueueName, false, false, false, null);
    }

    /// <summary>
    /// Cleans up the connections to the queue
    /// </summary>
    ~RabbitQueue()
    {
        connection?.Dispose();
        channel?.Dispose();
    }
}
