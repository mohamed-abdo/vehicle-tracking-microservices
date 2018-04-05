using BackgroundMiddleware.Concrete;
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

            Configuration = builder.Build();

            Environemnt = environemnt;

            logger
                .AddConsole()
                .AddDebug()
                .AddFile(configuration.GetSection("Logging"));

            Logger = logger
                .CreateLogger<Startup>();
            //local system configuration
            SystemLocalConfiguration = new ServiceConfiguration().Create(new Dictionary<string, string>() {
                {nameof(SystemLocalConfiguration.CacheServer), Configuration.GetValue<string>(Identifiers.CacheServer)},
                {nameof(SystemLocalConfiguration.VehiclesCacheDB),  Configuration.GetValue<string>(Identifiers.CacheDBVehicles)},
                {nameof(SystemLocalConfiguration.MessagesMiddleware),  Configuration.GetValue<string>(Identifiers.MessagesMiddleware)},
                {nameof(SystemLocalConfiguration.MiddlewareExchange),  Configuration.GetValue<string>(Identifiers.MiddlewareExchange)},
                {nameof(SystemLocalConfiguration.MessageSubscriberRoute),  Configuration.GetValue<string>(Identifiers.MessageSubscriberRoutes)},
                {nameof(SystemLocalConfiguration.MessagesMiddlewareUsername),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewareUsername)},
                {nameof(SystemLocalConfiguration.MessagesMiddlewarePassword),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewarePassword)},
            });
        }

        private MiddlewareConfiguration SystemLocalConfiguration;
        private RabbitMQPublisher MessagePublisher;
        private IOperationalUnit OperationalUnit;
        public IHostingEnvironment Environemnt { get; }
        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }
        public ICacheProvider Cache { get; set; }
        private string AssemblyName => $"{Environemnt.ApplicationName} V{this.GetType().Assembly.GetName().Version}";

        // Inject background service, for receiving message
        public void ConfigureServices(IServiceCollection services)
        {
            var loggerFactorySrv = services
                                    .BuildServiceProvider()
                                    .GetService<ILoggerFactory>();

            //add application insights information, could be used to monitor the performance, and more analytics when application moved to the cloud.
            loggerFactorySrv.AddApplicationInsights(services.BuildServiceProvider(), LogLevel.Information);

            ILogger _logger = loggerFactorySrv
                .AddConsole()
                .AddDebug()
                .CreateLogger<Startup>();

            OperationalUnit = new OperationalUnit(
                environment: Environemnt.EnvironmentName,
                assembly: AssemblyName);
            MessagePublisher = RabbitMQPublisher.Create(loggerFactorySrv, new RabbitMQConfiguration
            {
                hostName = SystemLocalConfiguration.MessagesMiddleware,
                exchange = SystemLocalConfiguration.MiddlewareExchange,
                userName = SystemLocalConfiguration.MessagesMiddlewareUsername,
                password = SystemLocalConfiguration.MessagesMiddlewarePassword,
                routes = new string[] { SystemLocalConfiguration.MessageSubscriberRoute }
            });
            // set cache service
            Cache = new CacheManager(Logger, SystemLocalConfiguration.CacheServer);
            services.AddOptions();

            #region worker background services

            #region ping worker
            services.AddSingleton<ICacheProvider, CacheManager>(srv => new CacheManager(
                _logger,
                SystemLocalConfiguration.CacheServer));

            services.AddSingleton<IHostedService, RabbitMQSubscriber<(MessageHeader, PingModel, MessageFooter)>>(srv =>
            {
                return RabbitMQSubscriber<(MessageHeader header, PingModel body, MessageFooter footer)>
                    .Create(loggerFactorySrv, new RabbitMQConfiguration
                    {
                        hostName = SystemLocalConfiguration.MessagesMiddleware,
                        exchange = SystemLocalConfiguration.MiddlewareExchange,
                        userName = SystemLocalConfiguration.MessagesMiddlewareUsername,
                        password = SystemLocalConfiguration.MessagesMiddlewarePassword,
                        routes = getRoutes("ping.vehicle")
                    }
                    , (pingMessageCallback) =>
                    {
                        try
                        {
                            var message = pingMessageCallback();
                            //cache model body by vehicle chassis as a key
                            if (message.body != null)
                            {
                                Cache
                                    .SetKey(message.body.ChassisNumber, Utilities.BinarySerialize(message.body))
                                    .Wait();
                            }
                            Logger.LogInformation($"[x] Tracking service received a message from exchange: {SystemLocalConfiguration.MiddlewareExchange}, route :{SystemLocalConfiguration.MessageSubscriberRoute}, message: {JsonConvert.SerializeObject(message)}");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogCritical(ex, "de-serialize Object exceptions.");
                        }
                    });
            });

            #region internal functions
            string[] getRoutes(string endwithMatch)
            {
                return SystemLocalConfiguration.MessageSubscriberRoute
                                 .Split(',')
                                               .Where(route => route.ToLower().EndsWith(endwithMatch, StringComparison.InvariantCultureIgnoreCase))
                                 .ToArray();
            }
            #endregion

            #endregion

            #endregion

            services.AddSingleton<IServiceLocator, ServiceLocator>(srv => new ServiceLocator(_logger, MessagePublisher, SystemLocalConfiguration, OperationalUnit));

            ///
            /// Injecting message receiver background service
            ///

            services.AddDistributedRedisCache(redisOptions =>
            {
                redisOptions.Configuration = SystemLocalConfiguration.CacheServer;
                redisOptions.InstanceName = SystemLocalConfiguration.VehiclesCacheDB;
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = OperationalUnit.Assembly, Version = "v1" });
            });


            services.AddMediatR();

            services.AddMvc(options =>
            {
                //TODO: add practical policy instead of empty policy for authentication / authorization .
                options.Filters.Add(new CustomAuthorizer(_logger, OperationalUnit));
                options.Filters.Add(new CustomeExceptoinHandler(_logger, OperationalUnit, Environemnt));
                options.Filters.Add(new CustomResponseResult(_logger, OperationalUnit));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IDistributedCache cache, IHostingEnvironment environemnt)
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
