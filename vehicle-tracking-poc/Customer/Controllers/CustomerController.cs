using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Services;
using Customer.Controllers.Mediator;
using Customer.Models;
using DomainModels.Business.CustomerDomain;
using DomainModels.Business.VehicleDomain;
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
        private readonly IMessageRequest<CustomerFilterModel, IEnumerable<DomainModels.Business.CustomerDomain.Customer>> _messageRequest;
        private readonly IMessageRequest<VehicleFilterModel, IEnumerable<Vehicle>> _vehicleMessageRequest;
        private readonly Guid _correlationId;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        private readonly IOperationalUnit _operationalUnit;
        public CustomerController(
            ILogger<CustomerController> logger,
            IMediator mediator,
            IMessageCommand messagePublisher,
            IMessageRequest<CustomerFilterModel, IEnumerable<DomainModels.Business.CustomerDomain.Customer>> messageRequest,
            IMessageRequest<VehicleFilterModel, IEnumerable<Vehicle>> vehicleMessageRequest,
        IOperationalUnit operationalUnit,
            MiddlewareConfiguration middlewareConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
            _messageRequest = messageRequest;
            _vehicleMessageRequest = vehicleMessageRequest;
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

        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.CustomerDomainModel)]
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] Models.CustomerRequest customerRequest, CancellationToken cancellationToken)
        {
            if (customerRequest == null)
                throw new ArgumentNullException("customer body request is missing!");
            await _mediator.Publish(new CustomerPublisher(
                            ControllerContext,
                            new DomainModels.Business.CustomerDomain.Customer()
                            {
                                Id = Guid.NewGuid(),
                                CorrelationId = _correlationId,
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

        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.CustomerResponseDomainModel)]
        [HttpGet("{correlationId}")]
        public async Task<IActionResult> Get(Guid correlationId)
        {
            //call mediator
            var request = new Mediator.CustomerRequest(
                    ControllerContext, new CustomerFilter
                    {
                        CorrelationId = correlationId
                    },
                     _messageRequest,
                     _vehicleMessageRequest,
                     _middlewareConfiguration,
                     _correlationId,
                     _operationalUnit
                );
            var result = await _mediator.Send(request);
            //transform the result model
            var trackingResult = result?.Select(t => new CustomerResponse(t)).FirstOrDefault();
            return new JsonResult(trackingResult);
        }
    }
}