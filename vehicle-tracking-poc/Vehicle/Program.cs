using System;
using BuildingAspects.Behaviors;
using DomainModels.System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Vehicle
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
                    var host = BuildWebHost(args);
                    host.Run();
                    return 0;
                });
            }
            catch (Exception ex)
            {
                mainLogger.LogCritical($"Failed to start ping vehicle service, {ex.Message}.", ex);
                return -1;
            }
            finally
            {
                mainLogger = null;
            }

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
