using BuildingAspects.Services;
using DomainModels.System;
using Microsoft.Extensions.Logging;

namespace BuildingAspects.Behaviors
{
    public interface IServiceMediator
    {
        ILogger Logger { get; }
        IOperationalUnit OperationalUnit { get; }
        IMessagePublisher MessagePublisher { get; }
        MiddlewareConfiguration MiddlewareConfiguration { get; }
    }
    public class ServiceMediator : IServiceMediator
    {
        private readonly ILogger _logger;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessagePublisher _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public ServiceMediator(
            ILogger logger,
            IMessagePublisher publisher,
            MiddlewareConfiguration localConfiguration,
            IOperationalUnit operationalUnit)
        {
            _logger = logger;
            _operationalUnit = operationalUnit;
            _publisher = publisher;
            _localConfiguration = localConfiguration;
        }

        public ILogger Logger => _logger;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessagePublisher MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
