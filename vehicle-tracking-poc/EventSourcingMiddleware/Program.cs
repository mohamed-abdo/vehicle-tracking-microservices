using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace MessagesMiddleware
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Running demo with Kestrel.");

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
                    options.Listen(IPAddress.Any, 5123); // docker outer port
                });

            var host = builder.Build();
            host.Run();
        }
    }
}
