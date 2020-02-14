using System;
using System.Collections.Generic;
using SimpleSearch.Storage.DocumentDb;

namespace SimpleSearch.Indexer.Shared.Entities
{
    public class TokenEntity : DbEntity
    {
        public string Tag { get; set; }

        public IEnumerable<DocumentEntity> Indexes { get; set; } = Array.Empty<DocumentEntity>();
    }
}