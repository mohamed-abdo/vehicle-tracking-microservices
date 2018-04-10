using BuildingAspects.Behaviors;
using BuildingAspects.Functors;
using BuildingAspects.Services;
using BuildingAspects.Utilities;
using DomainModels.DataStructure;
using DomainModels.Types;
using DomainModels.Types.Messages;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundMiddleware
{
    public class RabbitMQQueryWorker<TRequest, TResponse> : BackgroundService
    {
        private readonly ILogger logger;
        private int defaultMiddlewarePort = 5672;//default rabbitmq port
        private readonly RabbitMQConfiguration hostConfig;
        private readonly IConnectionFactory connectionFactory;

        private readonly Func<TRequest, TResponse> lambda;
        /// <summary>
        /// internal construct subscriber object
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="hostConfig">rabbitMQ configuration</param>
        private RabbitMQQueryWorker(ILoggerFactory logger, RabbitMQConfiguration hostConfig, Func<TRequest, TResponse> lambda)
        {
            this.logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQPublisher>()
                            ?? throw new ArgumentNullException("Logger reference is required");

            Validators.EnsureHostConfig(hostConfig);
            this.hostConfig = hostConfig;
            this.lambda = lambda ?? throw new ArgumentNullException("Callback reference is invalid");
            var host = Helper.ExtractHostStructure(this.hostConfig.hostName);
            connectionFactory = new ConnectionFactory() { HostName = host.hostName, Port = host.port ?? defaultMiddlewarePort, UserName = hostConfig.userName, Password = hostConfig.password, ContinuationTimeout = TimeSpan.FromSeconds(DomainModels.System.Identifiers.TimeoutInSec) };
        }

        /// <summary>
        /// factory constructor for subscriber object
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="hostConfig">rabbitMQ configuration</param>
        public static RabbitMQQueryWorker<TRequest, TResponse> Create(ILoggerFactory logger, RabbitMQConfiguration hostConfig, Func<TRequest, TResponse> lambda)
        {
            return new RabbitMQQueryWorker<TRequest, TResponse>(logger, hostConfig, lambda);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await new Function(logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
            {
                using (var connection = connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //TODO: in case scaling the middleware, running multiple workers simultaneously. 
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                    logger.LogInformation(" [x] Awaiting RPC requests");

                    consumer.Received += async (model, ea) =>
                    {
                        await new Function(logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
                        {
                            var props = ea.BasicProperties;
                            var replyProps = channel.CreateBasicProperties();
                            replyProps.CorrelationId = props.CorrelationId;

                            if (ea.Body == null || ea.Body.Length == 0)
                                throw new TypeLoadException("Invalid message type");
                            TResponse response = default(TResponse);
                            // callback action feeding 
                            if (Utilities.BinaryDeserialize(ea.Body) is TRequest request)
                                response = lambda(request);
                            else
                                throw new InvalidCastException("Invalid message cast");
                            //send acknowledgment to publisher

                            var responseBytes = Utilities.BinarySerialize(response);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                            logger.LogInformation($"[x] Event sourcing service receiving a messaged from exchange: {hostConfig.exchange}, route :{ea.RoutingKey}.");
                            return true;
                        }, (ex) =>
                        {
                            switch (ex)
                            {
                                case TypeLoadException typeEx:
                                    return true;
                                default:
                                    return false;
                            }
                        });
                    };
                    //bind event handler
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    Console.ReadLine();
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
