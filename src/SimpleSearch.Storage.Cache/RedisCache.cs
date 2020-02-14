using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace SimpleSearch.Storage.Cache
{
    public class RedisCache : IDistributedCache
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCache> _logger;

        public RedisCache(ILoggerFactory loggerFactory, ConnectionMultiplexer multiplexer)
        {
            _logger = loggerFactory.CreateLogger<RedisCache>();
            _database = multiplexer.GetDatabase();
        }

        public async Task<T> PassThroughCache<T>(string key, Func<Task<T>> action, CacheOptions options)
        {
            if (_database.KeyExists(key))
            {
                _logger.LogInformation($"Found key {key} in the cache");

                var value = await _database.StringGetAsync(key);

                return JsonConvert.DeserializeObject<T>(value);
            }

            _logger.LogInformation($"{key} was not found in the cache");

            var item = await action().ConfigureAwait(false);

            if (item != null)
            {
                await _database.StringSetAsync(key, JsonConvert.SerializeObject(item), options.Ttl);
            }

            return item;
        }
    }
}