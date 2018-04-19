using BuildingAspects.Services;
using Customer.Models;
using DomainModels.Business;
using DomainModels.Business.CustomerDomain;
using DomainModels.Business.VehicleDomain;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Customer.Controllers.Mediator
{
    public class CustomerRequest : IRequest<IEnumerable<DomainModels.Business.CustomerDomain.Customer>>
    {
        private readonly ControllerContext _controller;
        private readonly CustomerFilter _model;
        private readonly Guid _correlationId;
        private readonly IMessageRequest<CustomerFilterModel, IEnumerable<DomainModels.Business.CustomerDomain.Customer>> _request;
        private readonly IMessageRequest<VehicleFilterModel, IEnumerable<Vehicle>> _vehicleMessageRequest;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        private readonly IOperationalUnit _operationalUnit;

        public CustomerRequest(
            ControllerContext controller,
            CustomerFilter model,
            IMessageRequest<CustomerFilterModel, IEnumerable<DomainModels.Business.CustomerDomain.Customer>> request,
            IMessageRequest<VehicleFilterModel, IEnumerable<Vehicle>> vehicleMessageRequest,
            MiddlewareConfiguration middlewareConfiguration,
            Guid correlationId,
            IOperationalUnit operationalUnit)
        {
            _controller = controller;
            _model = model;
            _correlationId = correlationId;
            _operationalUnit = operationalUnit;
            _request = request;
            _vehicleMessageRequest = vehicleMessageRequest;
            _middlewareConfiguration = middlewareConfiguration;
        }
        public ControllerContext Controller => _controller;
        public CustomerFilter Model => _model;
        public IMessageRequest<CustomerFilterModel, IEnumerable<DomainModels.Business.CustomerDomain.Customer>> MessageRequest => _request;
        public IMessageRequest<VehicleFilterModel, IEnumerable<Vehicle>> VehicleMessageRequest => _vehicleMessageRequest;
        public Guid CorrelationId => _correlationId;
        public IOperationalUnit OperationalUnit => _operationalUnit;
        public MiddlewareConfiguration MiddlewareConfiguration => _middlewareConfiguration;
    }
}
