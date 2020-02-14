using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using SimpleSearch.Indexer.ClientResponses;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Storage.DocumentDb;

namespace SimpleSearch.Indexer.Application.Queries
{
    public class SearchDocumentsByTagQueryHandler : IRequestHandler<SearchDocumentsByTagQuery, SearchResponse>
    {
        private readonly IMongoDbContext<TokenEntity> _context;

        public SearchDocumentsByTagQueryHandler(IMongoDbContext<TokenEntity> context)
        {
            _context = context;
        }

        public async Task<SearchResponse> Handle(SearchDocumentsByTagQuery request, CancellationToken cancellationToken)
        {
            var normalizedTag = request.Tag.ToLowerInvariant();
            var cursor = await _context.Collection.FindAsync<TokenEntity>(
                Builders<TokenEntity>.Filter.Eq(x => x.Tag, normalizedTag), cancellationToken: cancellationToken);
            var token = cursor.FirstOrDefault();

            if (token == null)
            {
                return new SearchResponse {Documents = Array.Empty<Document>()};
            }

            return new SearchResponse
            {
                Documents = token.Indexes.Select(d => new Document {Extension = d.Extension, FileName = d.Name})
            };
        }
    }
}