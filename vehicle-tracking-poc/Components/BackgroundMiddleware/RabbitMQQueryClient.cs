﻿using System;
using System.Collections.Concurrent;
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
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BackgroundMiddleware
{
    /// <summary>
    /// rabbitMQ publisher manager
    /// </summary>
    public class RabbitMQQueryClient<TRequest, TResponse> : IMessageQuery<TRequest, TResponse>, IDisposable
    {
        private readonly ILogger logger;
        private int defaultMiddlewarePort = 5672;//default rabbitmq port
        private readonly RabbitMQConfiguration _hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<TResponse> respQueue = new BlockingCollection<TResponse>();
        private readonly IBasicProperties props;
        public readonly string exchange;
        private readonly string route;
        private RabbitMQQueryClient(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            this.logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQQueryClient<TRequest, TResponse>>()
                            ?? throw new ArgumentNullException("Logger reference is required");

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
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            props = channel.CreateBasicProperties();
            props.Persistent = true;
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                if (ea.Body == null)
                    return;
                if (!(Utilities.BinaryDeserialize(ea.Body) is TResponse response))
                    throw new InvalidCastException("Invalid message cast");
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }
        public static RabbitMQQueryClient<TRequest, TResponse> Create(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            return new RabbitMQQueryClient<TRequest, TResponse>(logger, hostConfig);
        }

        public async Task<TResponse> Query((MessageHeader Header, TRequest Body, MessageFooter Footer) message)
        {
            return await new Function(logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
              {
                  channel.BasicPublish(exchange: exchange,
                                       routingKey: route,
                                       basicProperties: props,
                                       body: Utilities.BinarySerialize(message));

                  logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", message.Header.ExecutionId, exchange, route);
                  channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);
                  return respQueue.Take();
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
