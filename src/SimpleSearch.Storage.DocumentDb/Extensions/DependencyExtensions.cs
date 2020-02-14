using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleSearch.Storage.DocumentDb.Extensions
{
    public static class DependencyExtensions
    {
        public static void AddMongoDbCollection<TEntity>(this IServiceCollection services, 
            string connectionString,
            string database,
            string collection) where TEntity : DbEntity
        {
            services.Configure<MongoDbSettings>(op =>
            {
                op.ConnectionString = connectionString;
                op.Database = database;
                op.Collection = collection;
            });

            services.AddSingleton<IMongoDbContext<TEntity>, MongoDbContext<TEntity>>();
        }
    }
}