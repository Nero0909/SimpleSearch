using Microsoft.Extensions.DependencyInjection;

namespace SimpleSearch.Storage.Blobs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlobStorage(this IServiceCollection services, string connectionString, string containerName)
        {
            services.Configure<BlobSettings>(op =>
            {
                op.ConnectionString = connectionString;
                op.ContainerName = containerName;
            });
            services.AddSingleton<IBlobStorage, AzureBlobStorage>();
        }
    }
}