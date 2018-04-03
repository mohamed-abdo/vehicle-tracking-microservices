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
    public class PingPublisher : ServiceLocator, INotification
    {
        private readonly IServiceLocator _locator;
        private readonly ControllerContext _controller;
        private readonly PingModel _model;
        public PingPublisher(
            IServiceLocator mediator,
            ControllerContext controller,
            PingModel model) : base(mediator.Logger, mediator.MessagePublisher, mediator.MiddlewareConfiguration, mediator.OperationalUnit)
        {
            _locator = mediator;
            _controller = controller;
            _model = model;
        }
        public IServiceLocator Locator => _locator;
        public ControllerContext Controller => _controller;
        public PingModel Model => _model;
    }
}
