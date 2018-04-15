using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ping.Contracts;
using Ping.Models;
using Ping.Vehicle.Mediator;
using WebComponents.Interceptors;

namespace Ping.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PingController : Controller, IPing
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMessageCommand _messagePublisher;
        private readonly IOperationalUnit _operationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public PingController(
            ILogger<PingController> logger,
            IMediator mediator,
            IMessageCommand messagePublisher,
            IOperationalUnit operationalUnit,
            MiddlewareConfiguration middlewareConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
            _operationalUnit = operationalUnit;
            _middlewareConfiguration = middlewareConfiguration;
        }

        // GET api/v/<controller>/0
        [CustomHeader("string", "print hello")]
        [HttpGet("{id?}")]
        public IActionResult Get(string id, CancellationToken cancellationToken)
        {
            return Ok($"ping service hello world;{id}");
        }

        // POST api/v/<controller>/vehicleId
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.PingDomainModel)]
        [HttpPost("{vehicleId}")]
        public async Task<IActionResult> Post(string vehicleId, PingRequest pingRequest, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new PingPublisher(
                            ControllerContext,
                            new DomainModels.Vehicle.Ping()
                            {
                                ChassisNumber = vehicleId,
                                Status = StatusModel.Active,
                                Message = "Hello world => vehicle"
                            },
                            _messagePublisher,
                            _middlewareConfiguration,
                            _operationalUnit), cancellationToken);
            return Ok();
        }
    }
}
