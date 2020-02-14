using System;

namespace SimpleSearch.Indexer.Application.Settings
{
    public class CacheSettings
    {
        public TimeSpan Ttl { get; set; } = TimeSpan.FromMinutes(1);
    }
}