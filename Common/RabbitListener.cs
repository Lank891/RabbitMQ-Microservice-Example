using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RabbitListener : BackgroundService
    {
        private readonly IConnection? connection;
        private readonly IModel? channel;

        private readonly ILogger<RabbitListener>? _logger;

        private readonly string[] _queueNames;

        private readonly ConcurrentQueue<byte[]> receivedMessages = new();

        /// <summary>
        /// Connects to the queue
        /// </summary>
        /// <param name="logger">Logger</param>
        public RabbitListener(string hostName, string[] queueNames, ILogger<RabbitListener>? logger = null)
        {
            _logger = logger;
            _queueNames = queueNames;

            ConnectionFactory? factory = new() { HostName = hostName };
            if (factory == null)
                throw new Exception("Could not create rabbit conection factory");

            connection = factory.CreateConnection();
            if (connection == null)
                throw new Exception("Could not create rabbit conection");

            channel = connection.CreateModel();
            if (channel == null)
                throw new Exception("Could not create rabbit channel");

            foreach(var queueName in _queueNames)
                channel.QueueDeclare(queueName, false, false, false, null);
        }

        /// <summary>
        /// Returns the oldest received message that was not returned already, if no more
        /// messages are avialable, it waits for the message (non-busy waiting). Due to concurrent
        /// nature it still might return null if message was received on another thread 
        /// </summary>
        /// <returns></returns>
        public byte[]? WaitAndGetNextMessage()
        {
            SpinWait.SpinUntil(() => !receivedMessages.IsEmpty);
            bool dequeueSuccess = receivedMessages.TryDequeue(out byte[]? message);

            return dequeueSuccess ? message : null;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                channel?.Dispose();
                connection?.Dispose();
                return Task.CompletedTask;
            }

            EventingBasicConsumer? consumer = new(channel);

            consumer.Received += (model, eventArgs) =>
            {
                _logger?.LogInformation("Received message on queue {queueName}", eventArgs.RoutingKey);
                receivedMessages.Enqueue(eventArgs.Body.ToArray());
            };

            foreach (var queueName in _queueNames)
                channel.BasicConsume(queueName, true, consumer);

            return Task.CompletedTask;
        }
    }
}
