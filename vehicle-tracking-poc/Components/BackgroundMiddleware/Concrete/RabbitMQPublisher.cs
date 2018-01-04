using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using BuildingAspects.Functors;
using DomainModels.DataStructure;
using DomainModels.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BackgroundMiddleware.Concrete
{
    /// <summary>
    /// rabbitMQ publisher manager
    /// </summary>
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly ILogger logger;
        private readonly RabbitMQConfiguration hostConfig;
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private IModel channel;
        private RabbitMQPublisher(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            this.logger = logger?
                            .AddConsole()
                            .AddDebug()
                            .CreateLogger<RabbitMQPublisher>()
                            ?? throw new ArgumentNullException("Logger reference is required");

            Validators.EnsureHostConfig(hostConfig);
            this.hostConfig = hostConfig;
            this.connectionFactory = new ConnectionFactory() { HostName = hostConfig.hostName, UserName = hostConfig.userName, Password = hostConfig.password };
        }
        public static RabbitMQPublisher Create(ILoggerFactory logger, RabbitMQConfiguration hostConfig)
        {
            return new RabbitMQPublisher(logger, hostConfig);
        }

        public Task Publish<T>(string exchange, string route, (MessageHeader Header, T Body, MessageFooter Footer) message)
        {
            try
            {
                using (var connection = connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    channel.BasicPublish(exchange: exchange,
                                         routingKey: route,
                                         basicProperties: properties,
                                         body: body);
                    logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", message.Header.ExecutionId, exchange, route);

                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message, ex);
                throw ex;
            }
            finally
            {

            }
        }
    }
}
