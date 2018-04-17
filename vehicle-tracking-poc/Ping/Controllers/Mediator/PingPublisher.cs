using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ping.Controllers.Mediator
{
    public class PingPublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly DomainModels.Business.Ping _model;
        private readonly Guid _correlationId;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public PingPublisher(
            ControllerContext controller,
            DomainModels.Business.Ping model,
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
            _localConfiguration = middlewareConfiguration;
        }
        public ControllerContext Controller => _controller;
        public DomainModels.Business.Ping Model => _model;
        public Guid CorrelationId => _correlationId;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
