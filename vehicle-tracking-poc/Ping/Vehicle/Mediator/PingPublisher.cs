using BuildingAspects.Behaviors;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ping.Vehicle.Mediator
{
    public class PingPublisher : ServiceLocator, INotification
    {
        private readonly IServiceLocator _services;
        private readonly ControllerContext _controller;
        private readonly PingModel _model;
        public PingPublisher(
            IServiceLocator services,
            ControllerContext controller,
            PingModel model) : base(services.Logger, services.MessagePublisher, services.MiddlewareConfiguration, services.OperationalUnit)
        {
            _services = services;
            _controller = controller;
            _model = model;
        }
        public IServiceLocator Servcies => _services;
        public ControllerContext Controller => _controller;
        public PingModel Model => _model;
    }
}
