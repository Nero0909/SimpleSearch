using RabbitMQ.Client;
using SimpleSearch.Messages;

namespace SimpleSearch.EventBus.RabbitMQ
{
    public class RabbitMQSubscriber : IEventSubscriber
    {
        private readonly IRabbitMQPersistentConnection _connection;

        public RabbitMQSubscriber(IRabbitMQPersistentConnection connection)
        {
            _connection = connection;
        }

        public void Subscribe<T>(string subscriptionName) where T : BaseMessage
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var eventName = typeof(T).Name;

            using var channel = _connection.CreateModel();
            channel.QueueDeclare(subscriptionName, durable: false, exclusive: false, autoDelete: false, null);
            channel.ExchangeDeclare(exchange: eventName, ExchangeType.Fanout, false, false, null);
            channel.QueueBind(
                queue: subscriptionName,
                exchange: eventName,
                routingKey: "", 
                null);
        }
    }
}