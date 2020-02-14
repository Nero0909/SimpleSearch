using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Storage.DocumentDb;

namespace SimpleSearch.Indexer.Shared
{
    public class MongoTokensRepository : ITokensRepository
    {
        private readonly MongoDbContext<TokenEntity> _context;

        public MongoTokensRepository(MongoDbContext<TokenEntity> context)
        {
            _context = context;
        }

        public Task AddDocumentToTokenAsync(DocumentEntity document, string tag, CancellationToken cancellationToken)
        {
            var id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            
            var filter = Builders<TokenEntity>.Filter.Where(x => x.Tag == tag);
            var update = Builders<TokenEntity>.Update.AddToSet(x => x.Indexes, document)
                .SetOnInsert(x => x.Id, id);

            return _context.Collection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true },
                cancellationToken);
        }

        public async Task<TokenEntity> FindByTagAsync(string tag, CancellationToken cancellationToken)
        {
            var cursor = await _context.Collection.FindAsync<TokenEntity>(
                Builders<TokenEntity>.Filter.Eq(x => x.Tag, tag), cancellationToken: cancellationToken);
            return cursor.FirstOrDefault();
        }
    }
}