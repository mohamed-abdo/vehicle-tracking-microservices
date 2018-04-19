using BuildingAspects.Services;
using DomainModels.Business;
using DomainModels.Business.VehicleDomain;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Vehicle.Controllers.Mediator
{
    public class VehicleRequest : IRequest<IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>>
    {
        private readonly ControllerContext _controller;
        private readonly VehicleFilter _model;
        private readonly Guid _correlationId;
        private readonly IMessageRequest<VehicleFilterModel, IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>> _request;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        private readonly IOperationalUnit _operationalUnit;

        public VehicleRequest(
            ControllerContext controller,
            VehicleFilter model,
            IMessageRequest<VehicleFilterModel, IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>> request,
            MiddlewareConfiguration middlewareConfiguration,
            Guid correlationId,
            IOperationalUnit operationalUnit)
        {
            _controller = controller;
            _model = model;
            _correlationId = correlationId;
            _operationalUnit = operationalUnit;
            _request = request;
            _middlewareConfiguration = middlewareConfiguration;
        }
        public ControllerContext Controller => _controller;
        public VehicleFilter Model => _model;
        public IMessageRequest<VehicleFilterModel, IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>> MessageRequest => _request;
        public Guid CorrelationId => _correlationId;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public MiddlewareConfiguration MiddlewareConfiguration => _middlewareConfiguration;
    }
}
