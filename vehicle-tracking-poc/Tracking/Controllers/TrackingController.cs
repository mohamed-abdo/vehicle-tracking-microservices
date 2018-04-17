using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Business;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisCacheAdapter;
using WebComponents.Interceptors;
using Tracking.Controllers.Mediator;
using System;

namespace Tracking.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TrackingController : Controller
    {
        private readonly ILogger _logger;
        private ICacheProvider _redisCache;
        private readonly IMediator _mediator;
        private readonly IMessageQuery<TrackingFilterModel, IEnumerable<PingModel>> _messageQuery;
        private readonly Guid _correlationId;
        private readonly IOperationalUnit _opertationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public TrackingController(
            ILogger<TrackingController> logger,
            IMediator mediator,
            ICacheProvider cache,
            IMessageQuery<TrackingFilterModel, IEnumerable<PingModel>> messageQuery,
            IOperationalUnit opertationalUnit,
            MiddlewareConfiguration middlewareConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _redisCache = cache;
            _messageQuery = messageQuery;
            _correlationId = Guid.NewGuid();
            _opertationalUnit = opertationalUnit;
            _middlewareConfiguration = middlewareConfiguration;
        }
        // GET api/values
        [CustomHeader("string", "print hello")]
        [HttpGet("{id?}")]
        public IActionResult Get(string id, CancellationToken cancellationToken)
        {
            return Ok($"tracking service hello world;{id}");
        }

        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.TrackingDomainModel)]
        [HttpGet("{startFrom}/{endBy}/{pageNo}/{pageSize}")]
        public async Task<IActionResult> Get(long startFrom, long endBy, int pageNo = 0, int pageSize = 10)
        {
            //call mediator
            var request = new TrackingRequest(
                    ControllerContext, new TrackingFilter
                    {
                        StartFromTime = startFrom,
                        EndByTime = endBy,
                        PageNo = pageNo,
                        rowsCount = pageSize
                    },
                    _redisCache,
                    _messageQuery,
                    _middlewareConfiguration,
                    _correlationId,
                    _opertationalUnit
                );
            var result = await _mediator.Send(request);
            return new JsonResult(result);
        }
    }
}
