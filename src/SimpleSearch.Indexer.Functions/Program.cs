using System;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleSearch.Indexer.Functions.Application.Infrastructure;
using SimpleSearch.Indexer.Shared;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Storage.DocumentDb.Extensions;

namespace SimpleSearch.Indexer.Functions
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .UseEnvironment("Development")
                .ConfigureAppConfiguration(config =>
                {
                    // Replace the configuration sources added by ConfigureWebJobs()
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebJobs(webJobsBuilder =>
                {
                    webJobsBuilder
                        .AddAzureStorageCoreServices()
                        .AddAzureStorage()
                        .AddRabbitMQ();
                })
                .ConfigureLogging(b =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetValue<string>("MongoDb:ConnectionString");
                    var database = context.Configuration.GetValue<string>("MongoDb:Database");
                    var collection = context.Configuration.GetValue<string>("MongoDb:Collection");

                    services.AddMongoDbCollection<TokenEntity>(connectionString, database, collection);
                    services.AddSingleton<ITokensRepository, MongoTokensRepository>();

                    services.AddSingleton<ITypeLocator, FunctionsTypeLocator>();

                    services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                var _ = (JobHost)host.Services.GetService<IJobHost>();

                // Wait for RabbitMQ
                await Task.Delay(TimeSpan.FromSeconds(5));

                await host.RunAsync();
            }
        }
    }
}
