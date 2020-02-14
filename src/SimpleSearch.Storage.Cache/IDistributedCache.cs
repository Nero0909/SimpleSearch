using System;
using System.Threading.Tasks;

namespace SimpleSearch.Storage.Cache
{
    public interface IDistributedCache
    {
        Task<T> PassThroughCache<T>(string key, Func<Task<T>> action, CacheOptions options);
    }
}