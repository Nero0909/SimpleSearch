using MediatR;

namespace SimpleSearch.Indexer.Application.Decorators
{
    public interface ICacheableRequest<out TResponse> : IRequest<TResponse>
    {
        string CacheKey { get; }
    }
}