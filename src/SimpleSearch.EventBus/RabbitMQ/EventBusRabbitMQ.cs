// Partly taken from https://github.com/dotnet-architecture/eShopOnContainers

using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using SimpleSearch.Messages;

namespace SimpleSearch.EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : IEventBus
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly RabbitMQSettings _settings;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IOptions<RabbitMQSettings> settings)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _settings = settings.Value;
        }

        public void Publish(BaseMessage message)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_settings.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", message.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = message.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", message.Id, eventName);

            using var channel = _persistentConnection.CreateModel();
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", message.Id);

            channel.ExchangeDeclare(exchange: eventName, ExchangeType.Fanout, false, false, null);

            var serializedMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(serializedMessage);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", message.Id);

                channel.BasicPublish(
                    exchange: eventName,
                    routingKey: "",
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }
    }
}