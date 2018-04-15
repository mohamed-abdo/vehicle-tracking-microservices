using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisCacheAdapter;
using Tracking.Tracking.Mediator;
using WebComponents.Interceptors;

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
        private readonly IOperationalUnit _oprtationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public TrackingController(
            ILogger<TrackingController> logger,
            IMediator mediator,
            ICacheProvider cache,
            IMessageQuery<TrackingFilterModel, IEnumerable<PingModel>> messageQuery,
            IOperationalUnit oprtationalUnit,
            MiddlewareConfiguration middlewareConfiguration)
        {
            _logger = logger;
            _mediator = mediator;
            _redisCache = cache;
            _messageQuery = messageQuery;
            _oprtationalUnit = oprtationalUnit;
            _middlewareConfiguration = middlewareConfiguration;
        }
        // GET api/values
        [CustomHeader("string", "print hello")]
        [HttpGet("{id?}")]
        public IActionResult Get(string id, CancellationToken cancellationToken)
        {
            return Ok($"tracking service hello world;{id}");
        }

        // GET api/values/5
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
                    _oprtationalUnit,
                    _middlewareConfiguration
                );
            var result = await _mediator.Send(request);
            return new JsonResult(result);
        }
    }
}
