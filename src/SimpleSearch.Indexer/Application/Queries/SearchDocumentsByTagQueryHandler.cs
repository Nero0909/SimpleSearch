using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleSearch.Indexer.ClientResponses;
using SimpleSearch.Indexer.Shared;

namespace SimpleSearch.Indexer.Application.Queries
{
    public class SearchDocumentsByTagQueryHandler : IRequestHandler<SearchDocumentsByTagQuery, SearchResponse>
    {
        private readonly ITokensRepository _tokens;

        public SearchDocumentsByTagQueryHandler(ITokensRepository tokens)
        {
            _tokens = tokens;
        }

        public async Task<SearchResponse> Handle(SearchDocumentsByTagQuery request, CancellationToken cancellationToken)
        {
            var normalizedTag = request.Tag.ToLowerInvariant();
            var token = await _tokens.FindByTagAsync(normalizedTag, cancellationToken);

            if (token == null)
            {
                return new SearchResponse {Documents = Array.Empty<Document>(), Tag = normalizedTag};
            }

            return new SearchResponse
            {
                Documents = token.Indexes.Select(d => new Document {Extension = d.Extension, FileName = d.Name}),
                Tag = normalizedTag
            };
        }
    }
}