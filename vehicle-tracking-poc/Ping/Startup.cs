using BackgroundMiddleware;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.DataStructure;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using WebComponents.Interceptors;

namespace Ping
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
                .AddFile("Logs/Startup-{Date}.txt", isJson: true);

            Logger = logger
                .CreateLogger<Startup>();
            //local system configuration
            SystemLocalConfiguration = new ServiceConfiguration().Create(new Dictionary<string, string>() {
                {nameof(SystemLocalConfiguration.CacheServer), Configuration.GetValue<string>(Identifiers.CacheServer)},
                {nameof(SystemLocalConfiguration.VehiclesCacheDB),  Configuration.GetValue<string>(Identifiers.CacheDBVehicles)},
                {nameof(SystemLocalConfiguration.MessagesMiddleware),  Configuration.GetValue<string>(Identifiers.MessagesMiddleware)},
                {nameof(SystemLocalConfiguration.MiddlewareExchange),  Configuration.GetValue<string>(Identifiers.MiddlewareExchange)},
                {nameof(SystemLocalConfiguration.MessagePublisherRoute),  Configuration.GetValue<string>(Identifiers.MessagePublisherRoute)},
                {nameof(SystemLocalConfiguration.MessagesMiddlewareUsername),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewareUsername)},
                {nameof(SystemLocalConfiguration.MessagesMiddlewarePassword),  Configuration.GetValue<string>(Identifiers.MessagesMiddlewarePassword)},
            });
        }

        private MiddlewareConfiguration SystemLocalConfiguration;
        private RabbitMQConfiguration rabbitMQConfiguration;
        private IOperationalUnit OperationalUnit;
        public IHostingEnvironment Environemnt { get; }
        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }
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
                .AddFile(Configuration.GetSection("Logging"))
                .CreateLogger<Startup>();

            OperationalUnit = new OperationalUnit(
                environment: Environemnt.EnvironmentName,
                assembly: AssemblyName);

            services.AddOptions();

            rabbitMQConfiguration = new RabbitMQConfiguration
            {
                hostName = SystemLocalConfiguration.MessagesMiddleware,
                exchange = SystemLocalConfiguration.MiddlewareExchange,
                userName = SystemLocalConfiguration.MessagesMiddlewareUsername,
                password = SystemLocalConfiguration.MessagesMiddlewarePassword,
                routes = new string[] { SystemLocalConfiguration.MessagePublisherRoute }
            };

            // no need to inject the following service since, currently they are injected for the mediator.

            services.AddTransient<IServiceLocator, ServiceLocator>(srv => new ServiceLocator(
                _logger,
                RabbitMQPublisher.Create(loggerFactorySrv, rabbitMQConfiguration),
                SystemLocalConfiguration,
                OperationalUnit));

            ///
            /// Injecting message receiver background service
            ///

            services.AddDistributedRedisCache(redisOptions =>
            {
                redisOptions.Configuration = SystemLocalConfiguration.CacheServer;
                redisOptions.Configuration = SystemLocalConfiguration.VehiclesCacheDB;
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
