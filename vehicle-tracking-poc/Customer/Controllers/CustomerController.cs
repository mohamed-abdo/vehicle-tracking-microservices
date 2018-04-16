using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Services;
using Customer.Controllers.Mediator;
using Customer.Models;
using DomainModels.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebComponents.Interceptors;

namespace Customer.Controllers
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
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.CustomerDomainModel)]
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] CustomerRequest customerRequest, CancellationToken cancellationToken)
        {
            if (customerRequest == null)
                throw new ArgumentNullException("customer body request is missing!");
            await _mediator.Publish(new CustomerPublisher(
                            ControllerContext,
                            new DomainModels.Business.Customer()
                            {
                                CustomerId = Guid.NewGuid(),
                                BirthDate = customerRequest.BirthDate,
                                Country = customerRequest.Country,
                                Email = customerRequest.Email,
                                Mobile = customerRequest.Mobile,
                                Name = customerRequest.Name
                            },
                            _messagePublisher,
                            _middlewareConfiguration,
                            _operationalUnit), cancellationToken);
            return Ok();
        }
    }
}