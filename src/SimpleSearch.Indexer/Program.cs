using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SimpleSearch.Indexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(GetConfiguration()).Run();
        }

        public static IWebHost CreateHostBuilder(IConfiguration configuration) =>
            new WebHostBuilder()
                .UseKestrel(opt =>
                {
                    opt.AllowSynchronousIO = false;
                    opt.AddServerHeader = false;
                })
                .ConfigureLogging(c => c.AddConsole())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseConfiguration(configuration)
                .Build();

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
