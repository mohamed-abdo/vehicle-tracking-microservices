using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Ping.Vehicle.Mediator
{
    public class PingPublisher : ServiceMediator, INotification
    {
        private readonly IServiceMediator _mediator;
        private readonly ControllerContext _controller;
        private readonly PingModel _model;
        public PingPublisher(
            IServiceMediator mediator,
            ControllerContext controller,
            PingModel model) : base(mediator.Logger, mediator.MessagePublisher, mediator.MiddlewareConfiguration, mediator.OperationalUnit)
        {
            _mediator = mediator;
            _controller = controller;
            _model = model;
        }
        public IServiceMediator Mediator => _mediator;
        public ControllerContext Controller => _controller;
        public PingModel Model => _model;
    }
}
