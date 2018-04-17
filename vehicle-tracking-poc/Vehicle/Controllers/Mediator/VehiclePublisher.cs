using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Vehicle.Controllers.Mediator
{
    public class VehiclePublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly DomainModels.Business.Vehicle _model;
        private readonly Guid _correlationId;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public VehiclePublisher(
            ControllerContext controller,
            DomainModels.Business.Vehicle model,
            IMessageCommand publisher,
            MiddlewareConfiguration middlewareConfiguration,
            Guid correlationId,
            IOperationalUnit operationalUnit)
        { 
            _controller = controller;
            _model = model;
            _correlationId = correlationId;
            _operationalUnit = operationalUnit;
            _publisher = publisher;
            _middlewareConfiguration = middlewareConfiguration;
        }
        public ControllerContext Controller => _controller;
        public DomainModels.Business.Vehicle Model => _model;
        public Guid CorrelationId => _correlationId;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _middlewareConfiguration;
    }
}
