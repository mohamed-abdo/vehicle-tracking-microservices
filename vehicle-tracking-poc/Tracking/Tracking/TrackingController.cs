using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingAspects.Behaviors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using RedisCacheAdapter;

namespace Tracking.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TrackingController : Controller
    {
        private readonly ILogger _logger;
        private ICacheProvider _redisCache;
        //private readonly IMediator _mediator;
        //private readonly IServiceLocator _servicesLocator;
        public TrackingController(
            ILogger<TrackingController> logger,
            ICacheProvider redisCache)
        {
            _logger = logger;
            _redisCache = redisCache;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            //sample
            var message = _redisCache.Get(id)?.Result;
            return message;
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
