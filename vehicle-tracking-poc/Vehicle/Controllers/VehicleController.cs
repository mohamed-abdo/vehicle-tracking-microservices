using System;
using System.Linq;
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
using DomainModels.Business.VehicleDomain;
using System.Collections.Generic;

namespace Vehicle.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VehicleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMessageCommand _messagePublisher;
        private readonly IMessageRequest<VehicleFilterModel, IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>> _messageRequest;
        private readonly Guid _correlationId;
        private readonly IOperationalUnit _operationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public VehicleController(
            ILogger<VehicleController> logger,
            IMediator mediator,
            IMessageCommand messagePublisher,
            IMessageRequest<VehicleFilterModel, IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>> messageRequest,
            IOperationalUnit operationalUnit,
            MiddlewareConfiguration middlewareConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
            _messageRequest = messageRequest;
            _correlationId = Guid.NewGuid();
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

        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.VehicleDomainModel)]
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] Models.VehicleRequest vehicleRequest, CancellationToken cancellationToken)
        {
            if (vehicleRequest == null)
                throw new ArgumentNullException("vehicle body request is missing!");
            await _mediator.Publish(new VehiclePublisher(
                            ControllerContext,
                            new DomainModels.Business.VehicleDomain.Vehicle()
                            {
                                Id = Guid.NewGuid(),
                                CorrelationId = _correlationId,
                                CustomerId = vehicleRequest.CustomerId,
                                //TODO:get customer name (referential data from cache)
                                //CustomerName = vehicleRequest.CustomerName,
                                ChassisNumber = vehicleRequest.ChassisNumber,
                                Color = vehicleRequest.Color,
                                Country = vehicleRequest.Country,
                                Model = vehicleRequest.Model,
                                ProductionYear = vehicleRequest.ProductionYear
                            },
                            _messagePublisher,
                            _middlewareConfiguration,
                            _correlationId,
                            _operationalUnit), cancellationToken);
            return Ok();
        }

        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.VehicleResponseDomainModel)]
        [HttpGet("{customerId}")]
        public async Task<IActionResult> Get(Guid customerId)
        {
            //call mediator
            var request = new Mediator.VehicleRequest(
                    ControllerContext
                    , new VehicleFilter
                    {
                        CustomerId = customerId
                    },
                     _messageRequest,
                     _middlewareConfiguration,
                     _correlationId,
                     _operationalUnit
                );
            var result = await _mediator.Send(request);
            //transform the result model
            var trackingResult = result?.Select(t => new VehicleResponse(t)).FirstOrDefault();
            return new JsonResult(trackingResult);
        }
    }
}

