using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace SimpleSearch.Storage.Cache.Extensions
{
    public static class DependencyExtensions
    {
        public static void AddRedis(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(sp =>
            {
                var configuration = ConfigurationOptions.Parse(connectionString, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<IDistributedCache, RedisCache>();
        }
    }
}