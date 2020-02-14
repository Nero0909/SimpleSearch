using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SimpleSearch.Analyzer.Functions.Application.Infrastructure;
using SimpleSearch.Analyzer.Functions.Application.Settings;
using SimpleSearch.Analyzer.Functions.Functions;
using SimpleSearch.EventBus;
using SimpleSearch.EventBus.RabbitMQ;
using SimpleSearch.EventBus.RabbitMQ.Extensions;
using SimpleSearch.Messages;
using SimpleSearch.Storage.Blobs.Extensions;

namespace SimpleSearch.Analyzer.Functions
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
                    services.AddSingleton<IEventSubscriber, RabbitMQSubscriber>();

                    var connectionString = context.Configuration.GetValue<string>("BlobStorage:ConnectionString");
                    var containerName = "files";
                    services.AddBlobStorage(connectionString, containerName);

                    services.AddSingleton<ITypeLocator, FunctionsTypeLocator>();

                    services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);

                    services.Configure<FragmentationSettings>(opt => opt.ChunkSizeInBytes = 300);
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                SubscribeToEvents(host.Services);
                var _ = (JobHost)host.Services.GetService<IJobHost>();

                await host.RunAsync();
            }
        }

        // Since RabbitMQ trigger can't subscribe dynamically,
        // we have to do the binding here 
        private static void SubscribeToEvents(IServiceProvider provider)
        {
            var subscriptionManager = provider.GetService<IEventSubscriber>();
            subscriptionManager.Subscribe<FileUploadedEvent>(SubscriptionName.Analyzer);
        }
    }
}
