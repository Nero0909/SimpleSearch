using System;
using System.Collections.Generic;

namespace SimpleSearch.Indexer.ClientResponses
{
    public class SearchResponse
    {
        public string Tag { get; set; }

        public IEnumerable<Document> Documents { get; set; } = Array.Empty<Document>();
    }
}