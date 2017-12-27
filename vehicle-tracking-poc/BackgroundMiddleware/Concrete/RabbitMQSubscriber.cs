using BackgroundMiddleware.Abstract;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundMiddleware.Concrete
{
    /// <summary>
    /// rabbitMQ worker listener background service. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RabbitMQSubscriber<T> : BackgroundService where T : struct, IDomainModel<T>
    {
        private readonly ILogger logger;
        private readonly IConnectionFactory connectionFactory;
        private readonly (string hostName, string userName, string password, string exchange, string[] routes) hostConfig;
        private RabbitMQSubscriber(ILogger logger, (string hostName, string userName, string password, string exchange, string[] routes) hostConfig)
        {
            this.logger = logger ?? throw new ArgumentNullException("Logger reference is required");
            this.hostConfig = hostConfig;
            var hostName = hostConfig.hostName ?? throw new ArgumentNullException("hostName reference is required");

            this.connectionFactory = new ConnectionFactory() { HostName = hostConfig.hostName, UserName = hostConfig.userName, Password = hostConfig.password };
        }
        public static RabbitMQSubscriber<T> Create(ILogger logger, (string hostName, string userName, string password, string exchange, string[] routes) hostConfig)
        {
            return new RabbitMQSubscriber<T>(logger, hostConfig);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
                var queueName = channel.QueueDeclare().QueueName;



                foreach (var bindingKey in hostConfig.routes)
                {
                    channel.QueueBind(queue: queueName,
                                      exchange: hostConfig.hostName,
                                      routingKey: bindingKey);
                }

                logger.LogInformation(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var encodedBody = Encoding.UTF8.GetString(body);
                    if (string.IsNullOrEmpty(encodedBody))
                        throw new TypeLoadException("Invalid message body");
                    var message = JsonConvert.DeserializeObject<T>(encodedBody);
                    var routingKey = ea.RoutingKey;
                    logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", message.Id, hostConfig.exchange, routingKey);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
                Console.ReadLine();

                return Task.CompletedTask;
            }
        }
    }
}
