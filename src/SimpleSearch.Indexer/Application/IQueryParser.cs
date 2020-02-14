using System.Collections.Generic;
using MediatR;
using SimpleSearch.Indexer.ClientRequests;
using SimpleSearch.Indexer.ClientResponses;

namespace SimpleSearch.Indexer.Application
{
    public interface IQueryParser
    {
        IRequest<SearchResponse> ParseQuery(SearchRequest request);
    }
}