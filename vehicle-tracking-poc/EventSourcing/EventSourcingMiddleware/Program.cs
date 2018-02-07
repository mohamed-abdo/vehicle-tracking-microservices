using System;
using System.Net;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using DomainModels.System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventSourcingMiddleware
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Running event sourcing middleware with Kestrel.");

            ILogger mainLogger = new LoggerFactory()
                                        .AddConsole()
										.AddFile("Logs/ProgramMain-{Date}.txt", isJson: true)
                                        .AddDebug()
                                        .CreateLogger<Program>();
            try
            {
                new Function(mainLogger, Identifiers.RetryCount).Decorate(() =>
                 {
                     var config = new ConfigurationBuilder()
                         .AddCommandLine(args)
                         .AddEnvironmentVariables()
						 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						 .Build();

                     var builder = new WebHostBuilder()
                            .UseConfiguration(config)
							.UseStartup<Startup>()
                            .UseKestrel(options =>
                            {
                                // TODO: support end-point for self checking, monitoring, administration, service / task cancellation.... 
                                options.Listen(IPAddress.Any, 80); // docker outer port
                            });

                     var host = builder.Build();
                     host.Run();
                     return Task.CompletedTask;
                 }).Wait();
                return 0;
            }
            catch (Exception ex)
            {
                mainLogger.LogCritical($"Failed to start the event sourcing middleware service, {ex.Message}.", ex);
                return -1;
            }
            finally
            {
                mainLogger = null;
            }
        }
    }
}
