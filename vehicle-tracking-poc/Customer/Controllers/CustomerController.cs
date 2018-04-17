using System;
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
    public class CustomerController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMessageCommand _messagePublisher;
        private readonly Guid _correlationId;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        private readonly IOperationalUnit _operationalUnit;
        public CustomerController(
            ILogger<CustomerController> logger,
            IMediator mediator,
            IMessageCommand messagePublisher,
            IOperationalUnit operationalUnit,
            MiddlewareConfiguration middlewareConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
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
                                Id = Guid.NewGuid(),
                                CorrelationId = _correlationId.ToString(),
                                Name = customerRequest.Name,
                                BirthDate = customerRequest.BirthDate,
                                Country = customerRequest.Country,
                                Email = customerRequest.Email,
                                Mobile = customerRequest.Mobile
                            },
                            _messagePublisher,
                            _middlewareConfiguration,
                            _correlationId,
                            _operationalUnit), cancellationToken);
            return Ok();
        }
    }
}