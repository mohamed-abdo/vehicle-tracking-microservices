using BackgroundMiddleware;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.DataStructure;
using DomainModels.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebComponents.Interceptors;
using Swashbuckle.AspNetCore.Swagger;
using MediatR;
using Microsoft.Extensions.Hosting;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using System;
using System.Linq;
using Newtonsoft.Json;
using RedisCacheAdapter;
using EventSourceingSqlDb.Adapters;
using EventSourceingSqlDb.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;

namespace Tracking
{
    public class Startup
    {
        public Startup(ILoggerFactory logger, IHostingEnvironment environemnt, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environemnt.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            _environemnt = environemnt;

            _logger = logger
                  .AddConsole()
                  .AddDebug()
                  .AddFile("Logs/Startup-{Date}.txt", isJson: true)
                  .CreateLogger<Startup>();

            //local system configuration
            _systemLocalConfiguration = new ServiceConfiguration().Create(new Dictionary<string, string>() {
                {nameof(_systemLocalConfiguration.CacheServer), Configuration.GetValue<string>(Identifiers.CacheServer)},
                {nameof(_systemLocalConfiguration.VehiclesCacheDB),  Configuration.GetValue<string>(Identifiers.CacheDBVehicles)},
                {nameof(_systemLocalConfiguration.EventDbConnection),  Configuration.GetValue<string>(Identifiers.EventDbConnection)},
                {nameof(_systemLocalConfiguration.MessagesMiddleware),  Configuration.GetValue<string>(Identifiers.MessagesMiddleware)},
                {nameof(_systemLocalConfiguration.MiddlewareExchange),  Configuration.GetValue<string>(Identifiers.MiddlewareExchange)},
                {nameof(_systemLocalConfiguration.MessageSubscriberRoute),  Configuration.GetValue<string>(Identifiers.MessageSubscriberRoutes)},
                {nameof(_systemLocalConfiguration.MessagesMiddlewareUsername),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewareUsername)},
                {nameof(_systemLocalConfiguration.MessagesMiddlewarePassword),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewarePassword)},
            });
        }

        private readonly MiddlewareConfiguration _systemLocalConfiguration;
        private readonly IHostingEnvironment _environemnt;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public IHostingEnvironment Environemnt => _environemnt;
        public IConfiguration Configuration => _configuration;
        public ILogger Logger => _logger;
        private string AssemblyName => $"{Environemnt.ApplicationName} V{this.GetType().Assembly.GetName().Version}";

        // Inject background service, for receiving message
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactorySrv = serviceProvider.GetService<ILoggerFactory>();

            services.AddDbContextPool<VehicleDbContext>(options => options.UseSqlServer(
              _systemLocalConfiguration.EventDbConnection,
              //enable connection resilience
              connectOptions =>
              {
                  connectOptions.EnableRetryOnFailure();
                  connectOptions.CommandTimeout(Identifiers.TimeoutInSec);
              })
          );

            //add application insights information, could be used to monitor the performance, and more analytics when application moved to the cloud.
            loggerFactorySrv.AddApplicationInsights(services.BuildServiceProvider(), LogLevel.Information);

            ILogger _logger = loggerFactorySrv
                .AddConsole()
                .AddDebug()
                .AddFile(Configuration.GetSection("Logging"))
                .CreateLogger<Startup>();

            var operationalUnit = new OperationalUnit(
                environment: Environemnt.EnvironmentName,
                assembly: AssemblyName);

            // set cache service for db index 1
            services.AddSingleton<ICacheProvider, CacheManager>(srv => new CacheManager(Logger, _systemLocalConfiguration.CacheServer, 1));
            services.AddSingleton<MiddlewareConfiguration, MiddlewareConfiguration>(srv => _systemLocalConfiguration);
            services.AddScoped<IOperationalUnit, IOperationalUnit>(srv => operationalUnit);
            services.AddOptions();

            #region worker background services


            #region tracking vehicle query client

            services.AddScoped<IMessageQuery<TrackingModel, IEnumerable<(MessageHeader, PingModel, MessageFooter)>>,
            RabbitMQQueryClient<TrackingModel, IEnumerable<(MessageHeader, PingModel, MessageFooter)>>>(
                srv =>
                {
                    return RabbitMQQueryClient<TrackingModel, IEnumerable<(MessageHeader, PingModel, MessageFooter)>>
                            .Create(loggerFactorySrv, new RabbitMQConfiguration
                            {
                                exchange = "",
                                hostName = _systemLocalConfiguration.MessagesMiddleware,
                                userName = _systemLocalConfiguration.MessagesMiddlewareUsername,
                                password = _systemLocalConfiguration.MessagesMiddlewarePassword,
                                routes = new string[] { "rpc_queue" },
                            });
                });

            #endregion



            #region build ping worker cache

            #region tracking query worker
            // business logic

            services.AddSingleton<IHostedService, RabbitMQQueryWorker<(MessageHeader header, TrackingModel body, MessageFooter footer), IEnumerable<(MessageHeader header, PingModel body, MessageFooter footer)>>>(srv =>
            {
                Func<(MessageHeader header, PingModel body, MessageFooter footer), bool> filterQuery = (model) =>
                {
                    return true;
                };
                //get pingService
                var pingSrv = new PingEventSourcingLedgerAdapter(loggerFactorySrv, srv.GetService<VehicleDbContext>());

                return RabbitMQQueryWorker<(MessageHeader header, TrackingModel body, MessageFooter footer), IEnumerable<(MessageHeader header, PingModel body, MessageFooter footer)>>
                .Create(serviceProvider, loggerFactorySrv, new RabbitMQConfiguration
                {
                    exchange = "",
                    hostName = _systemLocalConfiguration.MessagesMiddleware,
                    userName = _systemLocalConfiguration.MessagesMiddlewareUsername,
                    password = _systemLocalConfiguration.MessagesMiddlewarePassword,
                    routes = new string[] { "rpc_queue" },
                }
                , (trackingMessageRequest) =>
                {
                    try
                    {
                        //TODO: add business logic, result should be serializable
                        Logger.LogInformation($"[x] callback of RabbitMQQueryWorker=>, message: {JsonConvert.SerializeObject(trackingMessageRequest)}");
                        return pingSrv.Query(filterQuery)?.ToList();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCritical(ex, "de-serialize Object exceptions.");
                        return null;
                    }
                });
            });
            #endregion

            //you may get a different cache db, by passing db index parameter.
            #region cache worker

            services.AddSingleton<IHostedService, RabbitMQSubscriberWorker<(MessageHeader, PingModel, MessageFooter)>>(srv =>
            {
                var cache = new CacheManager(Logger, _systemLocalConfiguration.CacheServer, 1);
                return RabbitMQSubscriberWorker<(MessageHeader header, PingModel body, MessageFooter footer)>
                    .Create(serviceProvider, loggerFactorySrv, new RabbitMQConfiguration
                    {
                        hostName = _systemLocalConfiguration.MessagesMiddleware,
                        exchange = _systemLocalConfiguration.MiddlewareExchange,
                        userName = _systemLocalConfiguration.MessagesMiddlewareUsername,
                        password = _systemLocalConfiguration.MessagesMiddlewarePassword,
                        routes = new string[] { _systemLocalConfiguration.MessageSubscriberRoute }
                    }
                    , (pingMessageCallback) =>
                    {
                        try
                        {
                            var message = pingMessageCallback();
                            //cache model body by vehicle chassis as a key
                            if (message.body != null)
                            {
                                cache.Set(message.body.ChassisNumber, message.header.Timestamp.ToString(), Defaults.CacheTimeout).Wait();
                            }
                            Logger.LogInformation($"[x] Tracking service received a message from exchange: {_systemLocalConfiguration.MiddlewareExchange}, route :{_systemLocalConfiguration.MessageSubscriberRoute}, message: {JsonConvert.SerializeObject(message)}");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogCritical(ex, "de-serialize Object exceptions.");
                        }
                    });
            });

            #endregion

            #endregion


            #region internal functions

            #endregion


            #endregion

            ///
            /// Injecting message receiver background service
            ///

            services.AddDistributedRedisCache(redisOptions =>
            {
                redisOptions.Configuration = _systemLocalConfiguration.CacheServer;
                redisOptions.InstanceName = _systemLocalConfiguration.VehiclesCacheDB;
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = operationalUnit.Assembly, Version = "v1" });
            });


            services.AddMediatR();

            services.AddMvc(options =>
            {
                //TODO: add practical policy instead of empty policy for authentication / authorization .
                options.Filters.Add(new CustomAuthorizer(_logger, operationalUnit));
                options.Filters.Add(new CustomeExceptoinHandler(_logger, operationalUnit, Environemnt));
                options.Filters.Add(new CustomResponseResult(_logger, operationalUnit));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment environemnt)
        {
            app.UseStatusCodePages();
            if (environemnt.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            // Enable static files (if exists)
            app.UseStaticFiles();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", AssemblyName);
            });
            app.UseMvc();
        }
    }
}
