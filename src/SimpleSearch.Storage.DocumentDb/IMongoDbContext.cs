using MongoDB.Driver;

namespace SimpleSearch.Storage.DocumentDb
{
    public interface IMongoDbContext<TEntity> where TEntity : DbEntity
    {
        IMongoCollection<TEntity> Collection { get; }
    }
}