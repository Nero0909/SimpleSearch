using System;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleSearch.Indexer.Application;
using SimpleSearch.Indexer.Application.Decorators;
using SimpleSearch.Indexer.Application.Settings;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Storage.Cache.Extensions;
using SimpleSearch.Storage.DocumentDb.Extensions;

namespace SimpleSearch.Indexer
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

            services.AddSingleton<IQueryParser, QueryParser>();

            ConfigureDocumentDb(services);
            ConfigureCache(services);
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

            services.AddMongoDbCollection<TokenEntity>(connectionString, database, collection);
        }

        public void ConfigureCache(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("Redis:ConnectionString");
            var ttl = Configuration.GetValue<TimeSpan>("Redis:Ttl");

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>));
            services.Configure<CacheSettings>(opt => opt.Ttl = ttl);

            services.AddRedis(connectionString);
        }
    }
}
