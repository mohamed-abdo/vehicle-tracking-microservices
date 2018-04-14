using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using BuildingAspects.Functors;
using BuildingAspects.Services;
using BuildingAspects.Utilities;
using DomainModels.DataStructure;
using DomainModels.Types;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace BackgroundMiddleware
{
    /// <summary>
    /// rabbitMQ publisher manager
    /// </summary>
    public class RabbitMQPublisher : IMessageCommand, IDisposable
    {
        private readonly ILogger _logger;
        private int defaultMiddlewarePort = 5672;//default rabbitmq port
        private readonly RabbitMQConfiguration _hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;
        private readonly IBasicProperties props;
        public readonly string exchange;
        private readonly string route;
        public RabbitMQPublisher(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            this._logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQPublisher>()
                            ?? throw new ArgumentNullException("Logger reference is required");

            Validators.EnsureHostConfig(hostConfig);
            _hostConfig = hostConfig;
            exchange = _hostConfig.exchange;
            route = _hostConfig.routes.FirstOrDefault() ?? throw new ArgumentNullException("route queue is missing.");
            var host = Helper.ExtractHostStructure(_hostConfig.hostName);
            connectionFactory = new ConnectionFactory()
            {
                HostName = host.hostName,
                Port = host.port ?? defaultMiddlewarePort,
                UserName = _hostConfig.userName,
                Password = _hostConfig.password,
                ContinuationTimeout = TimeSpan.FromSeconds(DomainModels.System.Identifiers.TimeoutInSec)
            };
            new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
           {
               connection = connectionFactory.CreateConnection();
               channel = connection.CreateModel();
               return true;
           }, (ex) =>
           {
               switch (ex)
               {
                   case BrokerUnreachableException brokerEx:
                       return true;
                   case ConnectFailureException connEx:
                       return true;
                   case SocketException socketEx:
                       return true;
                   default:
                       return false;
               }
           }).Wait();
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);
            props = channel.CreateBasicProperties();
            props.Persistent = true;
        }

        public Task Command<TRequest>(TRequest message)
        {
            var body = Utilities.JsonBinarySerialize(message);
            channel.BasicPublish(exchange: exchange,
                                 routingKey: route,
                                 basicProperties: props,
                                 body: body);
            _logger.LogInformation("[x] Sent a message, exchange:{0}, route: {1}", exchange, route);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            if (connection != null && connection.IsOpen)
                connection.Close();
        }
    }
}
