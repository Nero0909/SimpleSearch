using System;
using System.Collections.Generic;

namespace SimpleSearch.Uploader.Application.Services
{
    public static class DistributionStrategy
    {
        public static IList<long> BySize(long total, long chunkSize)
        {
            if (chunkSize == 0)
            {
                return Array.Empty<long>();
            }

            if (total < chunkSize)
            {
                chunkSize = total;
            }

            var rest = total;
            var distribution = new List<long>();
            while (rest > 0)
            {
                distribution.Add(Math.Min(rest, chunkSize));
                rest -= chunkSize;
            }

            return distribution;
        }
    }
}
