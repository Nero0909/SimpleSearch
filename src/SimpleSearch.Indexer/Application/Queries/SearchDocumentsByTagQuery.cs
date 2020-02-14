using MediatR;
using SimpleSearch.Indexer.Application.Decorators;
using SimpleSearch.Indexer.ClientResponses;

namespace SimpleSearch.Indexer.Application.Queries
{
    public class SearchDocumentsByTagQuery : ICacheableRequest<SearchResponse>
    {
        public SearchDocumentsByTagQuery(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; }
        public string CacheKey => Tag;
    }
}