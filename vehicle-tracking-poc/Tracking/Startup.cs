using BackgroundMiddleware;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.DataStructure;
using DomainModels.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebComponents.Interceptors;
using Swashbuckle.AspNetCore.Swagger;
using MediatR;
using Microsoft.Extensions.Hosting;
using DomainModels.Business;
using System;
using System.Linq;
using Newtonsoft.Json;
using RedisCacheAdapter;
using EventSourceingSQLDB.Adapters;
using EventSourceingSQLDB.DbModels;
using Microsoft.EntityFrameworkCore;
using TrackingSQLDB.DbModels;
using TrackingSQLDB;

namespace Tracking
{
    public class Startup
    {
        private readonly MiddlewareConfiguration _systemLocalConfiguration;
        private readonly IHostingEnvironment _environemnt;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private string AssemblyName => $"{Environemnt.ApplicationName} V{this.GetType().Assembly.GetName().Version}";

        public IHostingEnvironment Environemnt => _environemnt;
        public IConfiguration Configuration => _configuration;
        public ILogger Logger => _logger;
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

        // Inject background service, for receiving message
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactorySrv = serviceProvider.GetService<ILoggerFactory>();

            services.AddDbContextPool<TrackingDbContext>(options => options.UseSqlServer(
              _systemLocalConfiguration.EventDbConnection,
              //enable connection resilience
              connectOptions =>
              {
                  connectOptions.EnableRetryOnFailure();
                  connectOptions.CommandTimeout(Identifiers.TimeoutInSec);
              })//.UseLoggerFactory(loggerFactorySrv)// to log queries
          );

            //add application insights information, could be used to monitor the performance, and more analytics when application moved to the cloud.
            loggerFactorySrv.AddApplicationInsights(services.BuildServiceProvider(), LogLevel.Information);

            ILogger _logger = loggerFactorySrv
                .AddConsole()
                .AddDebug()
                .AddFile(Configuration.GetSection("Logging"))
                .CreateLogger<Startup>();


            // set cache service for db index 1
            services.AddSingleton<ICacheProvider, CacheManager>(srv => new CacheManager(Logger, _systemLocalConfiguration.CacheServer));
            services.AddSingleton<MiddlewareConfiguration, MiddlewareConfiguration>(srv => _systemLocalConfiguration);
            services.AddScoped<IOperationalUnit, IOperationalUnit>(srv => new OperationalUnit(
                environment: Environemnt.EnvironmentName,
                assembly: AssemblyName));
            services.AddOptions();

            #region worker background services


            #region tracking vehicle query client

            services.AddScoped<IMessageQuery<TrackingFilterModel, IEnumerable<DomainModels.Business.Tracking>>,
            RabbitMQQueryClient<TrackingFilterModel, IEnumerable<DomainModels.Business.Tracking>>>(
                srv =>
                {
                    return new RabbitMQQueryClient<TrackingFilterModel, IEnumerable<DomainModels.Business.Tracking>>
                            (loggerFactorySrv, new RabbitMQConfiguration
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

            services.AddSingleton<IHostedService, RabbitMQQueryWorker>(srv =>
            {
                var trackingSrv = new TrackingManager(loggerFactorySrv, srv.GetService<TrackingDbContext>());

                return new RabbitMQQueryWorker
                (serviceProvider, loggerFactorySrv, new RabbitMQConfiguration
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
                        var trackingFilter = Utilities.JsonBinaryDeserialize<TrackingFilterModel>(trackingMessageRequest);
                        Logger.LogInformation($"[x] callback of RabbitMQQueryWorker=> a message");
                        var response = trackingSrv.Query(trackingFilter.Body, predicate: null)?.ToList();
                        if (response == null)
                            return new byte[0];
                        return Utilities.JsonBinarySerialize(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCritical(ex, "Object de-serialization exception.");
                        //to respond back to RPC client
                        return new byte[0];
                    }
                });
            });
            #endregion

            //you may get a different cache db, by passing db index parameter.
            #region cache workers

            //1- cache vehicle status (key/value)
            services.AddSingleton<IHostedService, RabbitMQSubscriberWorker>(srv =>
            {
                var trackingSrv = new TrackingManager(loggerFactorySrv, srv.GetService<TrackingDbContext>());
                var cacheSrv = new CacheManager(Logger, _systemLocalConfiguration.CacheServer);
                cacheSrv.Subscribe(Identifiers.onPing, (message) =>
                 {
                     // onPing callback
                     // add status & time stamp to vehicle 'set'
                     var pingModel = Utilities.JsonBinaryDeserialize<PingModel>(message);
                     if (pingModel?.Body != null)
                     {
                         cacheSrv.SetHash(pingModel.Body.ChassisNumber, new Dictionary<string, string> {
                                { pingModel.Header.Timestamp.ToString(), Enum.GetName(typeof(StatusModel), pingModel.Body.Status) }
                         }, Identifiers.cache_db_idx1).Wait();
                     }
                 });

                return new RabbitMQSubscriberWorker
                    (serviceProvider, loggerFactorySrv, new RabbitMQConfiguration
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
                            if (message != null)
                            {
                                //get ping model

                                var pingModel = Utilities.JsonBinaryDeserialize<PingModel>(message);
                                if (pingModel != null)
                                {
                                    //push vehicle ping on cache that expire after 1 minute, querying this cache db will have vehicles that ping within 1 minute
                                    cacheSrv.Set(pingModel.Body.ChassisNumber, pingModel.Header.Timestamp.ToString(), Defaults.CacheTimeout, Identifiers.cache_db_idx2)
                                         .Wait();
                                    //get relevant vehicle
                                    Vehicle vehicle = null;
                                    Customer customer = null;
                                    var vehicleBinary = cacheSrv.GetBinary(pingModel.Body.ChassisNumber)?.Result;
                                    if (vehicleBinary != null)
                                    {
                                        vehicle = Utilities.JsonBinaryDeserialize<Vehicle>(vehicleBinary);
                                        //get customer by vehicle
                                        var customerBinary = cacheSrv.GetBinary(vehicle.CustomerId.ToString())?.Result;
                                        if (customerBinary != null)
                                        {
                                            customer = Utilities.JsonBinaryDeserialize<Customer>(customerBinary);
                                        }
                                    }
                                    //build tracking object 
                                    var trackingObj = new DomainModels.Business.Tracking
                                    {
                                        CorrelationId = pingModel.Header.CorrelationId,
                                        ChassisNumber = vehicle?.ChassisNumber ?? pingModel.Body.ChassisNumber,
                                        Model = vehicle?.Model,
                                        Owner = customer?.Name,
                                        OwnerRef = (customer?.Id)?.ToString(),
                                        Status = pingModel.Body.Status,
                                        Message = pingModel.Body.Message,
                                        Timestamp = pingModel.Header.Timestamp,
                                    };
                                    //push tracking object to repository
                                    var trackingDbModel = new TrackingSQLDB.DbModels.Tracking(trackingObj);
                                    trackingSrv.Add(trackingDbModel).Wait();
                                    // publish on-ping cache event
                                    cacheSrv.Publish(Identifiers.onPing, message);
                                }
                            }
                            Logger.LogInformation($"[x] Tracking service received a message from exchange: {_systemLocalConfiguration.MiddlewareExchange}, route :{_systemLocalConfiguration.MessageSubscriberRoute}");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogCritical(ex, "Object de-serialization exception.");
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
                c.SwaggerDoc("v1", new Info { Title = AssemblyName, Version = "v1" });
            });


            services.AddMediatR();

            var _operationalUnit = new OperationalUnit(
               environment: Environemnt.EnvironmentName,
               assembly: AssemblyName);

            services.AddMvc(options =>
            {
                //TODO: add practical policy instead of empty policy for authentication / authorization .
                options.Filters.Add(new CustomAuthorizer(_logger, _operationalUnit));
                options.Filters.Add(new CustomeExceptoinHandler(_logger, _operationalUnit, Environemnt));
                options.Filters.Add(new CustomResponseResult(_logger, _operationalUnit));
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment environemnt)
        {
            // initialize InfoDbContext
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<TrackingDbContext>();
                dbContext?.Database?.EnsureCreated();
            }
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
