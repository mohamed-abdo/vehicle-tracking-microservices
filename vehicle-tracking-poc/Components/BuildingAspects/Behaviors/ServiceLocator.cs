using BuildingAspects.Services;
using DomainModels.System;
using Microsoft.Extensions.Logging;

namespace BuildingAspects.Behaviors
{
    public interface IServiceLocator
    {
        ILogger Logger { get; }
        IOperationalUnit OperationalUnit { get; }
        IMessageCommand MessagePublisher { get; }
        MiddlewareConfiguration MiddlewareConfiguration { get; }
    }
    public class ServiceLocator : IServiceLocator
    {
        private readonly ILogger _logger;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public ServiceLocator(
            ILogger logger,
            IMessageCommand publisher,
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
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
