using BackgroundMiddleware.Abstract;
using BackgroundMiddleware.Concrete;
using DomainModels.DataStructure;
using DomainModels.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace vehicleStatus
{
    public class Startup
    {
        public Startup(ILoggerFactory logger, IHostingEnvironment environemnt, IConfiguration configuration)
        {
            Logger = logger.CreateLogger<Startup>();
            Environemnt = environemnt;
            Configuration = configuration;
            //local system configuration
            SystemLocalConfiguration = LocalConfiguration.CreateSingletone(new Dictionary<string, string>() {
                {nameof(SystemLocalConfiguration.CacheServer), Configuration.GetValue<string>("distributed_cache")},
                {nameof(SystemLocalConfiguration.HTVehicles),  Configuration.GetValue<string>("vehicles")},
                {nameof(SystemLocalConfiguration.MessagesMiddleware),  Configuration.GetValue<string>("messages_middleware")},
                {nameof(SystemLocalConfiguration.MiddlewareExchange),  Configuration.GetValue<string>("middleware_exchange")},
                {nameof(SystemLocalConfiguration.MessagePublisherRoute),  Configuration.GetValue<string>("middleware_info_publisher")},
                {nameof(SystemLocalConfiguration.MessagesMiddlewareUsername),  Configuration.GetValue<string>("middleware_username")},
                {nameof(SystemLocalConfiguration.MessagesMiddlewarePassword),  Configuration.GetValue<string>("middleware_password")},
            });
        }

        private LocalConfiguration SystemLocalConfiguration;
        public IHostingEnvironment Environemnt { get; }
        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }


        // Inject background service, for receiving message
        public void ConfigureServices(IServiceCollection services)
        {
            var loggerFactorySrv = services.BuildServiceProvider().GetService<ILoggerFactory>();

            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>(srv =>
            {
                return RabbitMQPublisher.Create(loggerFactorySrv, new RabbitMQConfiguration
                {
                    hostName = SystemLocalConfiguration.MessagesMiddleware,
                    exchange = SystemLocalConfiguration.MiddlewareExchange,
                    userName = SystemLocalConfiguration.MessagesMiddlewareUsername,
                    password = SystemLocalConfiguration.MessagesMiddlewarePassword,
                    routes = new string[] { SystemLocalConfiguration.MessagePublisherRoute }
                });
            });

            ///
            /// Injecting message receiver background service
            ///

            services.AddDistributedRedisCache(redisOptions =>
            {
                redisOptions.Configuration = SystemLocalConfiguration.CacheServer;
                redisOptions.Configuration = SystemLocalConfiguration.HTVehicles;
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IDistributedCache cache, IHostingEnvironment environemnt)
        {
            if (environemnt.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "api/v1/{controller=vehicle}/{id?}");
                }
            );
        }
    }
}
