using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ping.Contracts;
using Ping.Models;
using Ping.Vehicle.Mediator;
using WebComponents.Interceptors;

namespace Ping
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VehicleController : Controller, IPing
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IServiceLocator _servicesLocator;
        public VehicleController(
            ILogger<VehicleController> logger,
            IMediator mediator,
            IServiceLocator servicesLocator)
        {
            _logger = logger;
            _mediator = mediator;
            _servicesLocator = servicesLocator;
        }

        // GET api/v/<controller>/5
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.PingDomainModel)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            //TODO:in the future in case of correlated action, link them by correlation header, ....       
            //[CustomHeader(Models.Identifiers.CorrelationId, _operationalUnit.OperationId)]

            // message definition
            //(MessageHeader Header,PingModel Body, MessageFooter Footer)
            await _mediator.Publish(new PingPublisher(_servicesLocator, ControllerContext, new PingModel()
            {
                ChassisNumber = id,
                Message = "Hello world => vehicle"
            }
            ), cancellationToken);
            //throw new CustomException(code: ExceptionCodes.MessageMalformed);
            return Ok(id);
        }

        // POST api/v/<controller>/vehicleId
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.PingDomainModel)]
        [HttpPost("{vehicleId}")]
        public async Task<IActionResult> Post(string vehicleId, PingRequest pingRequest, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new PingPublisher(_servicesLocator, ControllerContext, new PingModel() { Message = "Hello world => vehicle" }), cancellationToken);
            return Ok();
        }
    }
}
