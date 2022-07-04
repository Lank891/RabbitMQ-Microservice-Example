using Common;

namespace API.Rabbit;

/// <summary>
/// Interface for service sending messages to queue
/// </summary>
public interface IRabbitQueue
{
    /// <summary>
    /// Sends a message to a queue
    /// </summary>
    /// <param name="message">Message to be sent</param>
    void SendMessage(Message message);
}
