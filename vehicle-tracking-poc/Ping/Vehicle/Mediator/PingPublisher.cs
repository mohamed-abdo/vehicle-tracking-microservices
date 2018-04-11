using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ping.Vehicle.Mediator
{
    public class PingPublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly PingModel _model;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public PingPublisher(
            ControllerContext controller,
            PingModel model,
            IMessageCommand publisher,
            MiddlewareConfiguration localConfiguration,
            IOperationalUnit operationalUnit)
        { 
            _controller = controller;
            _model = model;
            _operationalUnit = operationalUnit;
            _publisher = publisher;
            _localConfiguration = localConfiguration;
        }
        public ControllerContext Controller => _controller;
        public PingModel Model => _model;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
