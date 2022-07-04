using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.ObjectModel;
using System.Text;

namespace Common
{
    public abstract class Message
    {
        /// <summary>
        /// Types of message
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]  
        public enum Type
        {
            Addition,
            Multiplication
        };

        /// <summary>
        /// Dictionary translating message type to queue name
        /// </summary>
        public static readonly ReadOnlyDictionary<Message.Type, string> QueueNameForMessageType = new(Utility.BuildDictionary(() =>
        {
            Dictionary<Message.Type, string> ret = new();
            foreach (Message.Type type in (Message.Type[])Enum.GetValues(typeof(Message.Type)))
                ret.Add(type, GetQueueNameForType(type));
            return ret;
        }));

        /// <summary>
        /// Dictionary translating queue name to message type
        /// </summary>
        public static readonly ReadOnlyDictionary<string, Message.Type> MessageTypeForQueueName = new(Utility.BuildDictionary(() =>
        {
            Dictionary<string, Message.Type> ret = new();
            foreach (Message.Type type in (Message.Type[])Enum.GetValues(typeof(Message.Type)))
                ret.Add(GetQueueNameForType(type), type);
            return ret;
        }));

        /// <summary>
        /// Get name of a queue for this message
        /// </summary>
        public string GetQueueName() => QueueNameForMessageType[GetMessageType()];

        private static string GetQueueNameForType(Message.Type type) => $"{type}Queue";

        /// <summary>
        /// Gets type of the message
        /// </summary>
        /// <returns></returns>
        public abstract Message.Type GetMessageType();

        /// <summary>
        /// Type for integrity checks
        /// </summary>
        [JsonProperty]
        private Message.Type integrityCheckType;

        /// <summary>
        /// Checks if type of message is the same as type declared in message
        /// </summary>
        /// <returns></returns>
        private bool CheckTypeIntegrity() => integrityCheckType == GetMessageType();

        /// <summary>
        /// Creates body for rabbit mq message from instace of the class
        /// </summary>
        /// <returns>Byte array from json serialization of this object</returns>
        public byte[] GetRabbitBody()
        {
            integrityCheckType = GetMessageType();
            string json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Creates instance of <see cref="T"/> from byte array of serialized json
        /// </summary>
        /// <typeparam name="T">Type of message</typeparam>
        /// <param name="rabbitBody">Byte array received from rabbit queue</param>
        /// <returns>Instance of <see cref="T"/> class</returns>
        public static T? CreateFromRabbitBody<T, K>(byte[] rabbitBody, ILogger<K> logger) where T : Message, new()
        {
            string json = Encoding.UTF8.GetString(rabbitBody);
            try
            {
                T? message = JsonConvert.DeserializeObject<T>(json);

                if (message == null)
                    throw new Exception();

                if(!message.CheckTypeIntegrity())
                    throw new Exception();

                logger?.LogInformation("Successfully read message as type {type}", typeof(T).Name);
                return message;
            }
            catch(Exception)
            {
                logger?.LogError("Could not read message as type {type}", typeof(T).Name);
                return null;
            }
        }
    }
}
