using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using BuildingAspects.Functors;
using BuildingAspects.Services;
using BuildingAspects.Utilities;
using DomainModels.DataStructure;
using DomainModels.Types.Messages;
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
        private readonly IConnection connection;
        private readonly IModel channel;
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
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

            props = channel.CreateBasicProperties();
            props.Persistent = true;

        }
   
        public async Task Command<TRequest>((MessageHeader Header, TRequest Body, MessageFooter Footer) message)
        {
            var body = Utilities.BinarySerialize(message);
            channel.BasicPublish(exchange: exchange,
                                 routingKey: route,
                                 basicProperties: props,
                                 body: body);
            _logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", message.Header.ExecutionId, exchange, route);

            await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
                    {
                        return Task.CompletedTask;
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
                    });
        }
        public void Dispose()
        {
            if (connection != null && connection.IsOpen)
                connection.Close();
        }
    }
}
