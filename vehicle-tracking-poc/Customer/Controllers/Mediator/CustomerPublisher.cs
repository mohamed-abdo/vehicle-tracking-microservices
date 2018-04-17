using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Customer.Controllers.Mediator
{
    public class CustomerPublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly DomainModels.Business.Customer _model;
        private readonly Guid _correlationId;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        private readonly IOperationalUnit _operationalUnit;

        public CustomerPublisher(
            ControllerContext controller,
            DomainModels.Business.Customer model,
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
        public DomainModels.Business.Customer Model => _model;
        public IMessageCommand MessagePublisher => _publisher;
        public Guid CorrelationId => _correlationId;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
