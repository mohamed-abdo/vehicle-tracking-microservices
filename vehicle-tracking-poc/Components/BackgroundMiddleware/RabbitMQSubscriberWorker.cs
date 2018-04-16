using BuildingAspects.Behaviors;
using BuildingAspects.Functors;
using BuildingAspects.Utilities;
using DomainModels.DataStructure;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundMiddleware
{
    /// <summary>
    /// rabbitMQ worker listener background service. 
    /// </summary>
    /// <typeparam name="TRequest">Expected structure coming from the publisher to the subscriber.</typeparam>
    public class RabbitMQSubscriberWorker : BackgroundService, IDisposable
    {
        private const int defaultMiddlewarePort = 5672;//default rabbitmq port
        private readonly ILogger _logger;
        private readonly RabbitMQConfiguration _hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;
        private readonly EventingBasicConsumer consumer;
        public readonly string exchange, route, queueName;
        //Design decision: keep/ delegate responsibility of translating and casting object to the target type, to receiver callback, even exception will be thrown in his execution thread.
        private readonly Action<Func<byte[]>> callback;
        /// <summary>
        /// internal construct subscriber object
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="hostConfig">rabbitMQ configuration</param>
        public RabbitMQSubscriberWorker(
            IServiceProvider serviceProvider,
            ILoggerFactory logger,
            RabbitMQConfiguration hostConfig,
            Action<Func<byte[]>> callback) : base(serviceProvider)
        {
            _logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQPublisher>()
                            ?? throw new ArgumentNullException("Logger reference is required");
            try
            {
                Validators.EnsureHostConfig(hostConfig);
                _hostConfig = hostConfig;
                exchange = _hostConfig.exchange;
                route = _hostConfig.routes.FirstOrDefault() ?? throw new ArgumentNullException("route queue is missing.");
                this.callback = callback ?? throw new ArgumentNullException("Callback reference is invalid");
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
                   channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

                   channel.ExchangeDeclare(exchange: _hostConfig.exchange, type: ExchangeType.Topic, durable: true);
                   //TODO: in case scaling the middleware, running multiple workers simultaneously. 
                   channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
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
               }).Wait();

                queueName = channel.QueueDeclare().QueueName;

                foreach (var bindingKey in _hostConfig.routes)
                {
                    channel.QueueBind(queue: queueName,
                                      exchange: _hostConfig.exchange,
                                      routingKey: bindingKey);
                }
                consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
                    {
                        if (ea.Body == null || ea.Body.Length == 0)
                            throw new TypeLoadException("Invalid message type");

                        // callback action feeding 
                        callback(() => ea.Body);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                        _logger.LogInformation($"[x] Event sourcing service receiving a messaged from exchange: {_hostConfig.exchange}, route :{ea.RoutingKey}.");
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
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to initialize RabbitMQSubscriberWorker", ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[*] Waiting for messages.");

            Console.ReadLine();
            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            if (connection != null && connection.IsOpen)
                connection.Close();
            base.Dispose();
        }
    }
}
