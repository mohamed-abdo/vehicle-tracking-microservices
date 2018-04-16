using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vehicle.Controllers.Mediator;
using Vehicle.Models;
using WebComponents.Interceptors;

namespace Vehicle.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VehicleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMessageCommand _messagePublisher;
        private readonly IOperationalUnit _operationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public VehicleController(
            ILogger<VehicleController> logger,
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
        // GET api/values
        [CustomHeader("string", "print hello")]
        [HttpGet("{id?}")]
        public IActionResult Get(string id, CancellationToken cancellationToken)
        {
            return Ok($"tracking service hello world;{id}");
        }

        // POST api/values
        // POST api/v/<controller>/vehicleId
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.VehicleDomainModel)]
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] VehicleRequest vehicleRequest, CancellationToken cancellationToken)
        {
            if (vehicleRequest == null)
                throw new ArgumentNullException("vehicle body request is missing!");
            await _mediator.Publish(new VehiclePublisher(
                            ControllerContext,
                            new DomainModels.Business.Vehicle()
                            {
                                Id=Guid.NewGuid(),
                                CorrelationId= _operationalUnit.InstanceId.ToString(),
                                ChassisNumber = vehicleRequest.ChassisNumber,
                                Color = vehicleRequest.Color,
                                Country = vehicleRequest.Country,
                                Model = vehicleRequest.Model,
                                ProductionYear = vehicleRequest.ProductionYear
                            },
                            _messagePublisher,
                            _middlewareConfiguration,
                            _operationalUnit), cancellationToken);
            return Ok();
        }
    }
}
