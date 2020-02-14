using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using SimpleSearch.Indexer.Functions.Application.Extensions;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Messages;
using SimpleSearch.Storage.DocumentDb;

namespace SimpleSearch.Indexer.Functions.Application.Commands
{
    public class IndexFragmentCommandHandler : IRequestHandler<IndexFragmentCommand, FragmentIndexedEvent>
    {
        private readonly IMongoDbContext<TokenEntity> _context;

        public IndexFragmentCommandHandler(IMongoDbContext<TokenEntity> context)
        {
            _context = context;
        }

        public async Task<FragmentIndexedEvent> Handle(IndexFragmentCommand request, CancellationToken cancellationToken)
        {
            await request.Tokens.Select(token => AddDocumentToToken(token, request, cancellationToken))
                .Throttle(Environment.ProcessorCount);

            return new FragmentIndexedEvent
            {
                FileName = request.FileName,
                UploadId = request.UploadId,
                Extension = request.Extension,
                Offset = request.Offset,
                Length = request.Length
            };
        }

        private Task<UpdateResult> AddDocumentToToken(Token token, IndexFragmentCommand command,
            CancellationToken cancellationToken)
        {
            var id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            var documentIndex = new DocumentEntity
                {Id = command.UploadId, Name = command.FileName, Extension = command.Extension.ToString()};

            var filter = Builders<TokenEntity>.Filter.Where(x => x.Tag == token.Tag);
            var update = Builders<TokenEntity>.Update.AddToSet(x => x.Indexes, documentIndex)
                .SetOnInsert(x => x.Id, id);

            return _context.Collection.UpdateOneAsync(filter, update, new UpdateOptions() {IsUpsert = true},
                cancellationToken);
        }
    }
}