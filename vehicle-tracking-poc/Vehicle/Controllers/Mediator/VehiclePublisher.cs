using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Vehicle.Controllers.Mediator
{
    public class VehiclePublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly DomainModels.Business.Vehicle _model;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public VehiclePublisher(
            ControllerContext controller,
            DomainModels.Business.Vehicle model,
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
        public DomainModels.Business.Vehicle Model => _model;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
