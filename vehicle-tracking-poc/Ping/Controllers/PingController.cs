using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Business;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ping.Contracts;
using Ping.Models;
using WebComponents.Interceptors;
using Ping.Controllers.Mediator;
using Ping.Helper;
using System;

namespace Ping.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PingController : Controller, IPing
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMessageCommand _messagePublisher;
        private readonly Guid _correlationId;
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
            _correlationId = Guid.NewGuid();
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
        // Design decision, keep vehicleId part of endpoint, since body is optional and on the most of cases will be dropped.
        // POST api/v/<controller>/vehicleId
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.PingDomainModel)]
        [HttpPost("{vehicleId}")]
        public async Task<IActionResult> Post(string vehicleId, [FromBody] PingRequest pingRequest, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new PingPublisher(
                            ControllerContext,
                            new DomainModels.Business.Ping()
                            {
                                ChassisNumber = vehicleId,
                                Status = Mappers.inferStatus(pingRequest?.Status),
                                Message = pingRequest?.Message
                            },
                            _messagePublisher,
                            _middlewareConfiguration,
                            _correlationId,
                            _operationalUnit), cancellationToken);
            return Ok();
        }
    }
}
