using System;
using System.Net;
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
                                        .AddDebug()
                                        .CreateLogger<Program>();
            try
            {
                return new Function(mainLogger, Identifiers.RetryCount).Decorate(() =>
                {
                    var config = new ConfigurationBuilder()
                        .AddCommandLine(args)
                        .AddEnvironmentVariables()
                        .Build();

                    var builder = new WebHostBuilder()
                        .UseConfiguration(config)
                        .UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            // TODO: support end-point for self checking, monitoring, administration, service / task cancellation.... 
                            options.Listen(IPAddress.Any, 5553); // docker outer port
                        });

                    var host = builder.Build();
                    host.Run();
                    return 0;
                });
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
