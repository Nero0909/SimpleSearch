using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleSearch.Indexer.Application.Settings;
using SimpleSearch.Storage.Cache;

namespace SimpleSearch.Indexer.Application.Decorators
{
    public class CacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<CacheBehaviour<TRequest, TResponse>> _logger;
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _settings;

        public CacheBehaviour(ILogger<CacheBehaviour<TRequest, TResponse>> logger, IDistributedCache cache,
            IOptions<CacheSettings> settings)
        {
            _logger = logger;
            _cache = cache;
            _settings = settings.Value;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICacheableRequest<TResponse> cacheableRequest)
            {
                var cacheKey = cacheableRequest.CacheKey;
                return await _cache.PassThroughCache(cacheKey, () => next(), new CacheOptions(_settings.Ttl));
            }

            return await next();
        }
    }
}