using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagesMiddleware
{
    internal class Startup
    {
            public Startup(IHostingEnvironment env = null)
            {
                Configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();
            }

            public IConfiguration Configuration { get; private set; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                //background messaging service, will listen to all events to store them in event source queue.
                //services.AddSingleton<IHostedService, MessagesServiceHost>(srv =>
                //{
                //    var logger = srv.GetService<ILoggerFactory>();
                //    logger.AddConsole((message, logLevel) => logLevel >= LogLevel.Information, true);
                //    return new MessagesServiceHost(logger);
                //});
                return services.BuildServiceProvider();
            }

            public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
            {
                loggerFactory.AddConsole();

                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();

                app.Run(async (context) =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response
                        .WriteAsync("<p>Messages Middleware, Hosted by Kestrel </p>");

                    if (serverAddressesFeature != null)
                    {
                        await context.Response
                            .WriteAsync("<p>Listening on the following addresses: " +
                                string.Join(", ", serverAddressesFeature.Addresses) +
                                "</p>");
                    }

                    await context.Response.WriteAsync($"<p>Request URL: {context.Request.GetDisplayUrl()} </p>");
                });
            }

        }

    }
