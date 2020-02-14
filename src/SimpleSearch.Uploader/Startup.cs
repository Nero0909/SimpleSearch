using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleSearch.EventBus;
using SimpleSearch.EventBus.RabbitMQ;
using SimpleSearch.EventBus.RabbitMQ.Extensions;
using SimpleSearch.Storage.Blobs.Extensions;
using SimpleSearch.Storage.DocumentDb.Extensions;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Settings;

namespace SimpleSearch.Uploader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.Configure<UploadSettings>(opt =>
            {
                opt.ChunkSizeInBytes = Configuration.GetValue<long>("Upload:ChunkSizeInBytes");
            });

            ConfigureDocumentDb(services);
            ConfigureBlobStorage(services);
            ConfigureEventBus(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureDocumentDb(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("MongoDb:ConnectionString");
            var database = Configuration.GetValue<string>("MongoDb:Database");
            var collection = Configuration.GetValue<string>("MongoDb:Collection");

            services.AddMongoDbCollection<UploadSession>(connectionString, database, collection);
        }

        public void ConfigureEventBus(IServiceCollection services)
        {
            var hostName = Configuration.GetValue<string>("RabbitMQ:Host");
            var userName = Configuration.GetValue<string>("RabbitMQ:User");
            var password = Configuration.GetValue<string>("RabbitMQ:Password");

            services.AddRabitMQConnection(hostName, userName, password);
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
        }

        public void ConfigureBlobStorage(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("BlobStorage:ConnectionString");
            var containerName = "files";
            services.AddBlobStorage(connectionString, containerName);
        }
    }
}
