using System;

namespace SimpleSearch.Storage.Cache
{
    public class CacheOptions
    {
        public CacheOptions(TimeSpan ttl)
        {
            Ttl = ttl;
        }

        public TimeSpan Ttl { get; }
    }
}