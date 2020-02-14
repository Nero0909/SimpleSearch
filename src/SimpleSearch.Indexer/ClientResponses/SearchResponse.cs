using System;
using System.Collections.Generic;

namespace SimpleSearch.Indexer.ClientResponses
{
    public class SearchResponse
    {
        public IEnumerable<Document> Documents { get; set; } = Array.Empty<Document>();
    }
}