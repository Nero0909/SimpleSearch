using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace SimpleSearch.EventBus.RabbitMQ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabitMQConnection(this IServiceCollection services, string host, string user,
            string password, int retryCount = 5)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory
                {
                    HostName = host,
                    UserName = user,
                    Password = password,
                    DispatchConsumersAsync = true
                };

                return new RabbitMQPersistentConnection(factory, logger);
            });

            services.Configure<RabbitMQSettings>(opt => { opt.RetryCount = retryCount; });
        }
    }
}