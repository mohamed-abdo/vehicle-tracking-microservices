using BackgroundMiddleware.Abstract;
using BuildingAspects.Functors;
using DomainModels.DataStructure;
using DomainModels.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundMiddleware.Concrete
{
    /// <summary>
    /// rabbitMQ worker listener background service. 
    /// </summary>
    /// <typeparam name="T">Expected structure coming from the publisher to the subscriber.</typeparam>
    public class RabbitMQSubscriber<T> : BackgroundService
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;
        private readonly Action<(MessageHeader Header, T Body, MessageFooter Footer)> callback;
        /// <summary>
        /// internal construct subscriber object
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="hostConfig">rabbitMQ configuration</param>
        private RabbitMQSubscriber(ILoggerFactory logger, RabbitMQConfiguration hostConfig, Action<(MessageHeader Header, T Body, MessageFooter Footer)> callback)
        {
            this.logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQPublisher>()
                            ?? throw new ArgumentNullException("Logger reference is required");

            Validators.EnsureHostConfig(hostConfig);
            this.hostConfig = hostConfig;
            this.callback = callback ?? throw new ArgumentNullException("Callback reference is invalid");
            this.connectionFactory = new ConnectionFactory() { HostName = hostConfig.hostName, UserName = hostConfig.userName, Password = hostConfig.password };
        }

        /// <summary>
        /// factory constructor for subscriber object
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        /// <param name="hostConfig">rabbitMQ configuration</param>
        public static RabbitMQSubscriber<T> Create(ILoggerFactory logger, RabbitMQConfiguration hostConfig, Action<(MessageHeader Header, T Body, MessageFooter Footer)> callback)
        {
            return new RabbitMQSubscriber<T>(logger, hostConfig, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var connection = connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    channel.ExchangeDeclare(exchange: hostConfig.exchange, type: ExchangeType.Topic, durable: true);

                    var queueName = channel.QueueDeclare().QueueName;

                    foreach (var bindingKey in hostConfig.routes)
                    {
                        channel.QueueBind(queue: queueName,
                                          exchange: hostConfig.exchange,
                                          routingKey: bindingKey);
                    }

                    logger.LogInformation("[*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var messageBody = ea.Body;
                            var messageStr = Encoding.UTF8.GetString(messageBody);
                            if (string.IsNullOrEmpty(messageStr))
                                throw new TypeLoadException("Invalid message type");

                            var message = JsonConvert.DeserializeObject<(MessageHeader Header, T Body, MessageFooter Footer)>(messageStr);

                            // callback action feeding
                            callback(message);
                            //send acknowledgment to publisher
                            var routingKey = ea.RoutingKey;
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                            logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", "ExecutionId", hostConfig.exchange, routingKey);
                        }
                        catch (Exception ex)
                        {
                            logger.LogCritical(ex.Message, ex);
                            throw ex;
                        }
                    };
                    channel.BasicConsume(queue: queueName,
                                         autoAck: false,
                                         consumer: consumer);
                    Console.ReadLine();

                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.IsOpen)
                    connection.Close();
            }
        }
    }
}

