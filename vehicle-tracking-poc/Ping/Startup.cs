using BackgroundMiddleware.Abstract;
using BackgroundMiddleware.Concrete;
using DomainModels.DataStructure;
using DomainModels.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ping.Models;

namespace vehicleStatus
{
    public class Startup
    {
        #region config keys

        private const string _cacheServer = "distributed_cache";
        private const string _messagesMiddleware = "messages_middleware";
        private const string _HTVehicles = "vehicles";

        private const string _middlewareExchange = "platform3";
        private const string _messagePuplicherRoute = "info.ping.vehicle";
        private const string _messageSubscriberRoute = "info.ping.vehicle";
        private const string _username = "guest";
        private const string _password = "guest";
        #endregion

        public Startup(ILoggerFactory logger, IHostingEnvironment environemnt, IConfiguration configuration)
        {
            Logger = logger.CreateLogger<Startup>();
            Environemnt = environemnt;
            Configuration = configuration;
        }

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
                    hostName = _messagesMiddleware,
                    exchange = _middlewareExchange,
                    userName = _username,
                    password = _password,
                    routes = new string[] { _messagePuplicherRoute }
                });
            });

            ///
            /// Injecting message receiver background service
            ///
            services.AddSingleton<IHostedService, RabbitMQSubscriber<DomainModel<PingRequest>>>(srv =>
            {
                return RabbitMQSubscriber<DomainModel<PingRequest>>.Create(loggerFactorySrv,
                    new RabbitMQConfiguration
                    {
                        hostName = _messagesMiddleware,
                        exchange = _middlewareExchange,
                        userName = _username,
                        password = _password,
                        routes = new string[] { _messageSubscriberRoute }
                    }
                    , (pingMessage) =>
                    {
                        Logger.LogInformation($"[x] Ping service receiving a message-Id: {pingMessage.Header.ExecutionId} from exchange: {_middlewareExchange}, route :{_messageSubscriberRoute}, message: {JsonConvert.SerializeObject(pingMessage)}");
                    });
            });

            services.AddDistributedRedisCache(redisOptions =>
            {
                redisOptions.Configuration = _cacheServer;
                redisOptions.Configuration = _HTVehicles;
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
