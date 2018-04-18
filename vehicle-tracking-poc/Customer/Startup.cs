using BackgroundMiddleware;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using CustomerSQLDB;
using CustomerSQLDB.DbModels;
using DomainModels.Business;
using DomainModels.DataStructure;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedisCacheAdapter;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using WebComponents.Interceptors;
namespace Customer
{
    public class Startup
    {
        private readonly MiddlewareConfiguration _systemLocalConfiguration;
        private readonly IHostingEnvironment _environemnt;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public IHostingEnvironment Environemnt => _environemnt;
        public IConfiguration Configuration => _configuration;
        public ILogger Logger => _logger;

        private string AssemblyName => $"{Environemnt.ApplicationName} V{this.GetType().Assembly.GetName().Version}";

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
                {nameof(_systemLocalConfiguration.MessagePublisherRoute),  Configuration.GetValue<string>(Identifiers.MessagePublisherRoute)},
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

            services.AddDbContextPool<CustomerDbContext>(options => options.UseSqlServer(
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



            // no need to inject the following service since, currently they are injected for the mediator.

            services.AddSingleton<MiddlewareConfiguration, MiddlewareConfiguration>(srv => _systemLocalConfiguration);

            services.AddScoped<IOperationalUnit, IOperationalUnit>(srv => new OperationalUnit(
                environment: Environemnt.EnvironmentName,
                assembly: AssemblyName));

            services.AddScoped<IMessageCommand, RabbitMQPublisher>(srv => new RabbitMQPublisher(loggerFactorySrv,
            new RabbitMQConfiguration
            {
                hostName = _systemLocalConfiguration.MessagesMiddleware,
                exchange = _systemLocalConfiguration.MiddlewareExchange,
                userName = _systemLocalConfiguration.MessagesMiddlewareUsername,
                password = _systemLocalConfiguration.MessagesMiddlewarePassword,
                routes = new string[] { _systemLocalConfiguration.MessagePublisherRoute }
            }));
            services.AddOptions();

            #region worker

            #region customer worker

            services.AddSingleton<IHostedService, RabbitMQSubscriberWorker>(srv =>
            {
                //get Vehicle service
                var customerSrv = new CustomerManager(loggerFactorySrv, srv.GetService<CustomerDbContext>());
                var cacheSrv = new CacheManager(Logger, _systemLocalConfiguration.CacheServer);
                return new RabbitMQSubscriberWorker
                (serviceProvider, loggerFactorySrv, new RabbitMQConfiguration
                {
                    hostName = _systemLocalConfiguration.MessagesMiddleware,
                    exchange = _systemLocalConfiguration.MiddlewareExchange,
                    userName = _systemLocalConfiguration.MessagesMiddlewareUsername,
                    password = _systemLocalConfiguration.MessagesMiddlewarePassword,
                    routes = _systemLocalConfiguration.MessageSubscriberRoute?.Split('-') ?? new string[0]
                }
                    , (messageCallback) =>
                    {
                        try
                        {
                            var message = messageCallback();
                            if (message != null)
                            {
                                var domainModel = Utilities.JsonBinaryDeserialize<CustomerModel>(message);
                                var customer = new CustomerSQLDB.DbModels.Customer(domainModel.Body);
                                customerSrv.Add(customer).Wait();
                                cacheSrv.SetBinary(customer.Id.ToString(), Utilities.JsonBinarySerialize(customer)).Wait();
                            }
                            Logger.LogInformation($"[x] Customer service receiving a message from exchange: {_systemLocalConfiguration.MiddlewareExchange}, route :{_systemLocalConfiguration.MessageSubscriberRoute}");
                        }
                        catch (System.Exception ex)
                        {
                            Logger.LogCritical(ex, "Object de-serialization exception.");
                        }
                    });
            });

            #endregion

            #endregion

            ///
            /// Injecting message receiver background service
            ///

            services.AddDistributedRedisCache(redisOptions =>
            {
                redisOptions.Configuration = _systemLocalConfiguration.CacheServer;
                redisOptions.Configuration = _systemLocalConfiguration.VehiclesCacheDB;
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
        public void Configure(IApplicationBuilder app, IDistributedCache cache, IHostingEnvironment environemnt)
        {
            // initialize InfoDbContext
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<CustomerDbContext>();
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
