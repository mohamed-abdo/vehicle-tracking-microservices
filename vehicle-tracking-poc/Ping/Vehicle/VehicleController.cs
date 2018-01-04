using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using DomainModels.System;
using DomainModels.Types;
using DomainModels.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ping
{
    [Route("api/v1/[controller]")]
    public class VehicleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMessagePublisher _publisher;
        private readonly LocalConfiguration _localConfiguration;
        public VehicleController(ILogger<VehicleController> logger, IMessagePublisher publisher, LocalConfiguration localConfiguration)
        {
            _logger = logger;
            _publisher = publisher;
            _localConfiguration = localConfiguration;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            // message definition
            //(MessageHeader Header, DomainModel<PingRequest> Body, MessageFooter Footer)
            Task.Run(() => _publisher.Publish(
                _localConfiguration.MiddlewareExchange,
                _localConfiguration.MessagePublisherRoute,
                (
                    Header: new MessageHeader { ExecutionId = Guid.NewGuid(), CorrelateId = Guid.Empty, Timestamp = 1010 },
                    Body: new DomainModel<PingModel>()
                    {
                        Model = new PingModel() { VehicelId = Guid.NewGuid(), StatusDescription = "ping - pong!" },
                        Name = "me @abdo! domain model"
                    },
                    Footer: new MessageFooter { Route = _localConfiguration.MessagePublisherRoute }
                )));

            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
