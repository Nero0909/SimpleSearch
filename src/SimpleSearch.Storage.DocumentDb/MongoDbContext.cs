using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SimpleSearch.Storage.DocumentDb
{
    public class MongoDbContext<TEntity> where TEntity : DbEntity
    {
        private readonly IMongoDatabase _database;
        private readonly string _collection;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);

            _database = client.GetDatabase(settings.Value.Database);
            _collection = settings.Value.Collection;
        }

        public IMongoCollection<TEntity> Collection => _database.GetCollection<TEntity>(_collection);
    }
}