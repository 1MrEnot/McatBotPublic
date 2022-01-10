namespace McatBot.LongPolling
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Ioc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;

    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            var host = CreateHost();

            var app = ActivatorUtilities.CreateInstance<LongPollApp>(host.Services);
            await app.Run();
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            var aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{aspnetEnv}.json", true, true)
                .AddEnvironmentVariables();
        }

        public static IHost CreateHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddLogging();
                    services.AddApi();
                    services.AddHandlers();
                    services.AddLinkSearchers();

                    services.AddSingleton<LongPollApp>();
                    services.AddSingleton<LongPollEventProducer>();
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSimpleConsole(options =>
                    {
                        options.TimestampFormat = "[dd-MM-yyyy HH:mm:ss] ";
                        options.ColorBehavior = LoggerColorBehavior.Enabled;
                        options.IncludeScopes = true;
                    });
                    loggingBuilder.AddFile("logs\\log-{Date}.txt",
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message} {NewLine}{Exception}");
                })
                .Build();
        }
    }
}