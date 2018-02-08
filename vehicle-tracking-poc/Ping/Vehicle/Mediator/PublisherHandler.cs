using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ping.Vehicle.Mediator
{
    public class PublisherHandler : INotificationHandler<Publisher>
    {
        private readonly ILogger<PublisherHandler> _logger;
        public PublisherHandler(ILogger<PublisherHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(Publisher notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Publish notification=> {notification.Model}");
            await Task.CompletedTask;
        }
    }
}
