using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using DomainModels.System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;

namespace Tracking
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Running vehicle service with Kestrel.");

            ILogger mainLogger = new LoggerFactory()
                                        .AddConsole()
                                        .AddDebug()
                                        .AddFile("Logs/ProgramMain-{Date}.txt", isJson: true)
                                        .CreateLogger<Program>();
            try
            {
                new Function(mainLogger, Identifiers.RetryCount).Decorate(() =>
                {
                    BuildWebHost(args)
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
                    return Task.CompletedTask;
                }, (ex) =>
                {
                    switch (ex)
                    {
                        case BrokerUnreachableException brokerEx:
                            return true;
                        case ConnectFailureException connEx:
                            return true;
                        case SocketException socketEx:
                            return true;
                        default:
                            return false;
                    }
                }).Wait();
                return 0;
            }
            catch (Exception ex)
            {
                mainLogger.LogCritical($"Failed to start tracking vehicle service, {ex.Message}.", ex);
                return -1;
            }
            finally
            {
                mainLogger = null;
            }
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory());
    }
}
