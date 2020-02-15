using System;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleSearch.EventBus.RabbitMQ;
using SimpleSearch.EventBus.RabbitMQ.Extensions;
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
                    var hostName = context.Configuration.GetValue<string>("RabbitMQ:Host");
                    var userName = context.Configuration.GetValue<string>("RabbitMQ:User");
                    var password = context.Configuration.GetValue<string>("RabbitMQ:Password");

                    services.AddRabitMQConnection(hostName, userName, password);

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

                var connection = host.Services.GetService<IRabbitMQPersistentConnection>();
                connection.TryConnect();

                await host.RunAsync();
            }
        }
    }
}
