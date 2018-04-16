using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Controllers.Mediator
{
    public class CustomerPublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly DomainModels.Business.Customer _model;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public CustomerPublisher(
            ControllerContext controller,
            DomainModels.Business.Customer model,
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
        public DomainModels.Business.Customer Model => _model;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
