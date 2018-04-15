﻿using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ping.Controllers.Mediator
{
    public class PingPublisher : INotification
    {
        private readonly ControllerContext _controller;
        private readonly DomainModels.Business.Ping _model;
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessageCommand _publisher;
        private readonly MiddlewareConfiguration _localConfiguration;
        public PingPublisher(
            ControllerContext controller,
            DomainModels.Business.Ping model,
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
        public DomainModels.Business.Ping Model => _model;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public IMessageCommand MessagePublisher => _publisher;
        public MiddlewareConfiguration MiddlewareConfiguration => _localConfiguration;
    }
}
