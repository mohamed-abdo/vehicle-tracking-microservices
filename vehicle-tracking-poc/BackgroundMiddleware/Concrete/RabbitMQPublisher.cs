using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using DomainModels.System;
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
        private readonly string hostName;
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private readonly IModel channel;
        private RabbitMQPublisher(ILogger logger, string hostName, string userName = null, string password = null)
        {
            this.logger = logger ?? throw new ArgumentNullException("Logger reference is required");
            this.hostName = hostName ?? throw new ArgumentNullException("hostName reference is required");

            this.connectionFactory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password };
            this.connection = connectionFactory?.CreateConnection();

            this.channel = connection.CreateModel();
        }
        public static RabbitMQPublisher Create(ILogger logger, string hostName, string userName = null, string password = null)
        {
            return new RabbitMQPublisher(logger, hostName, userName, password);
        }

        public void Publish<T>(string exchange, string route, T message) where T : struct, IDomainModel<T>
        {
            verifyConnection();
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: exchange,
                                 routingKey: route,
                                 basicProperties: properties,
                                 body: body);
            logger.LogInformation("[x] Sent a message {0}, exchange:{1}, route: {2}", message.Id, exchange, route);
        }

        void verifyConnection()
        {
            if (!connection.IsOpen)
                connection = connectionFactory?.CreateConnection();
        }
    }
}
