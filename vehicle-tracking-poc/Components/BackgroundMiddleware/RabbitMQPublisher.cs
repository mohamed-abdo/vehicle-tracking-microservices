using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using BuildingAspects.Functors;
using BuildingAspects.Services;
using BuildingAspects.Utilities;
using DomainModels.DataStructure;
using DomainModels.Types;
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
        private readonly ILogger logger;
        private int defaultMiddlewarePort = 5672;//default rabbitmq port
        private readonly RabbitMQConfiguration hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
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
            connectionFactory = new ConnectionFactory()
            {
                HostName = host.hostName,
                Port = host.port ?? defaultMiddlewarePort,
                UserName = hostConfig.userName,
                Password = hostConfig.password,
                ContinuationTimeout = TimeSpan.FromSeconds(DomainModels.System.Identifiers.TimeoutInSec)
            };
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
        }
        public static RabbitMQPublisher Create(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            return new RabbitMQPublisher(logger, hostConfig);
        }

        public async Task Command<TRequest>(string exchange, string route, (MessageHeader Header, TRequest Body, MessageFooter Footer) message)
        {
            await new Function(logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
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
                    connection.Close();
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
