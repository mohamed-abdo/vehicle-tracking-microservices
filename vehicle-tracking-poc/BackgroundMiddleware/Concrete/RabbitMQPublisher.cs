using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Components.BackgroundMiddleware.Abstract;
namespace Components.BackgroundMiddleware.Concrete
{
    public class RabbitMQPublisher : BackgroundService
    {

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
