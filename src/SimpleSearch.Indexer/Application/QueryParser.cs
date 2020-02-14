using MediatR;
using SimpleSearch.Indexer.Application.Queries;
using SimpleSearch.Indexer.ClientRequests;
using SimpleSearch.Indexer.ClientResponses;

namespace SimpleSearch.Indexer.Application
{
    public class QueryParser : IQueryParser
    {
        public IRequest<SearchResponse> ParseQuery(SearchRequest request)
        {
            if (request.Query?.Tag != null)
            {
                return new SearchDocumentsByTagQuery(request.Query.Tag);
            }

            return null;
        }
    }
}