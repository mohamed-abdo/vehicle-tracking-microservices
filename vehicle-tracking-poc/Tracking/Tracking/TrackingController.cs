using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisCacheAdapter;
using Tracking.Tracking.Mediator;

namespace Tracking.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TrackingController : Controller
    {
        private readonly ILogger _logger;
        private ICacheProvider _redisCache;
        private readonly IMediator _mediator;
        private readonly IMessageQuery<TrackingModel, IEnumerable<(MessageHeader, TrackingModel, MessageFooter)>> _messageQuery;
        private readonly IOperationalUnit _oprtationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public TrackingController(
            ILogger<TrackingController> logger,
            IMediator mediator,
            ICacheProvider cache,
            IMessageQuery<TrackingModel, IEnumerable<(MessageHeader, TrackingModel, MessageFooter)>> messageQuery,
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
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            //call mediator
            var request = new TrackingRequest(
                    ControllerContext,
                    new TrackingModel { },
                    _redisCache,
                    _messageQuery,
                    _oprtationalUnit,
                    _middlewareConfiguration
                );
            var result = await _mediator.Send(request);
            return new JsonResult(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
