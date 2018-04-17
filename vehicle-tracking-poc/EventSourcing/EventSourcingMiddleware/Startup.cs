using BackgroundMiddleware;
using BuildingAspects.Behaviors;
using DomainModels.DataStructure;
using DomainModels.System;
using EventSourceingSQLDB.DbModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using EventSourceingSQLDB.Repository;

namespace EventSourcingMiddleware
{
    internal class Startup
    {
        public Startup(ILoggerFactory logger, IHostingEnvironment environemnt, IConfiguration configuration)
        {
            _configuration = configuration;

            _environemnt = environemnt;

            _logger = new LoggerFactory()
                .AddConsole()
                .AddDebug()
                .AddFile(configuration.GetSection("Logging"))
                .CreateLogger<Startup>();

            //local system configuration
            _systemLocalConfiguration = new ServiceConfiguration().Create(new Dictionary<string, string>() {

                {nameof(_systemLocalConfiguration.CacheServer), Configuration.GetValue<string>(Identifiers.CacheServer)},
                {nameof(_systemLocalConfiguration.VehiclesCacheDB),  Configuration.GetValue<string>(Identifiers.CacheDBVehicles)},
                {nameof(_systemLocalConfiguration.MessagesMiddleware),  Configuration.GetValue<string>(Identifiers.MessagesMiddleware)},
                {nameof(_systemLocalConfiguration.MiddlewareExchange),  Configuration.GetValue<string>(Identifiers.MiddlewareExchange)},
                {nameof(_systemLocalConfiguration.MessageSubscriberRoute),  Configuration.GetValue<string>(Identifiers.MessageSubscriberRoutes)},
                {nameof(_systemLocalConfiguration.MessagesMiddlewareUsername),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewareUsername)},
                {nameof(_systemLocalConfiguration.MessagesMiddlewarePassword),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewarePassword)},
                {nameof(_systemLocalConfiguration.EventDbConnection),    Configuration.GetValue<string>(Identifiers.EventDbConnection)},
            });
        }

        private readonly MiddlewareConfiguration _systemLocalConfiguration;
        private readonly IHostingEnvironment _environemnt;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public IHostingEnvironment Environemnt => _environemnt;
        public IConfiguration Configuration => _configuration;
        public ILogger Logger => _logger;
        // Inject background service, for receiving message
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactorySrv = serviceProvider.GetService<ILoggerFactory>();

            services.AddDbContextPool<EventSourcingDbContext>(options => options.UseSqlServer(
                _systemLocalConfiguration.EventDbConnection,
                //enable connection resilience
                connectOptions =>
                {
                    connectOptions.EnableRetryOnFailure();
                    connectOptions.CommandTimeout(Identifiers.TimeoutInSec);
                })//.UseLoggerFactory(loggerFactorySrv)// to log queries
            );
            /// Injecting message receiver background service
            ///
            #region worker background services

            #region event sourcing worker

            services.AddSingleton<IHostedService, RabbitMQSubscriberWorker>(srv =>
            {
                //get pingServicek
                var eventSourcingSrv = new EventSourcingLedger(loggerFactorySrv, srv.GetService<EventSourcingDbContext>());

                return new RabbitMQSubscriberWorker
                (serviceProvider, loggerFactorySrv, new RabbitMQConfiguration
                {
                    hostName = _systemLocalConfiguration.MessagesMiddleware,
                    exchange = _systemLocalConfiguration.MiddlewareExchange,
                    userName = _systemLocalConfiguration.MessagesMiddlewareUsername,
                    password = _systemLocalConfiguration.MessagesMiddlewarePassword,
                    routes = _systemLocalConfiguration.MessageSubscriberRoute?.Split('-') ?? new string[0]
                }
                    , (pingMessageCallback) =>
                    {
                        try
                        {
                            var message = pingMessageCallback();
                            if (message != null)
                            {
                                var domainModel = Utilities.JsonBinaryDeserialize<DomainModels.Types.DomainModel<object>>(message);
                                var dbModel = DbModelFactory.Create(domainModel);
                                if (dbModel != null)
                                {
                                    eventSourcingSrv.Add(dbModel).Wait();
                                }
                            }
                            Logger.LogInformation($"[x] Event sourcing service receiving a message from exchange: {_systemLocalConfiguration.MiddlewareExchange}, route :{_systemLocalConfiguration.MessageSubscriberRoute}");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogCritical(ex, "Object de-serialization exception.");
                        }
                    });
            });

            #endregion

            #endregion

        }
        #region internal functions

        #endregion

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // initialize InfoDbContext
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<EventSourcingDbContext>();
                dbContext?.Database?.EnsureCreated();
            }

            loggerFactory.AddConsole();
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response
                    .WriteAsync("<p>Event sourcing middleware service, Hosted by Kestrel </p>");

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
