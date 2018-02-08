using DomainModels.Vehicle;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ping.Vehicle.Mediator
{
    public class Publisher : INotification
    {
        private readonly PingModel _model;
        public Publisher(PingModel model)
        {
            _model = model;
        }
        public PingModel Model => _model;
    }
}
