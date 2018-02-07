using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using BuildingAspects.Behaviors;
using BuildingAspects.Functors;
using BuildingAspects.Utilities;
using DomainModels.DataStructure;
using DomainModels.System;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace BackgroundMiddleware.Concrete
{
    /// <summary>
    /// rabbitMQ publisher manager
    /// </summary>
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly ILogger logger;
        private int defaultMiddlewarePort = 5672;//default rabbitmq port
        private readonly RabbitMQConfiguration hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private RabbitMQPublisher(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            this.logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQPublisher>()
                            ?? throw new ArgumentNullException("Logger reference is required");

            Validators.EnsureHostConfig(hostConfig);
            this.hostConfig = hostConfig;
            var host = Helper.ExtractHostStructure(this.hostConfig.hostName);
            connectionFactory = new ConnectionFactory() { HostName = host.hostName, Port = host.port ?? defaultMiddlewarePort, UserName = hostConfig.userName, Password = hostConfig.password, ContinuationTimeout = TimeSpan.FromSeconds(DomainModels.System.Identifiers.TimeoutInSec) };
        }
        public static RabbitMQPublisher Create(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            return new RabbitMQPublisher(logger, hostConfig);
        }

        public async Task Publish<T>(string exchange, string route, (MessageHeader Header, T Body, MessageFooter Footer) message) where T : DomainModel
        {
            await new Function(logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
                {
                    if (connection == null || !connection.IsOpen)
                        connection = connectionFactory.CreateConnection();
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        var body = Utilities.BinarySerialize(message);
                        channel.BasicPublish(exchange: exchange,
                                             routingKey: route,
                                             basicProperties: properties,
                                             body: body);
                        logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", message.Header.ExecutionId, exchange, route);

                        return Task.CompletedTask;
                    }
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
    }
}
