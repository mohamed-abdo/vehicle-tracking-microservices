using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using DomainModels.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ping.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ping
{
    [Route("api/v1/[controller]")]
    public class VehicleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMessagePublisher _publisher;
        public VehicleController(ILogger<VehicleController> logger, IMessagePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
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
            const string _middlewareExchange = "platform3";
            const string _messagePuplicherRoute = "info.ping.vehicle";

            Task.Run(() => _publisher.Publish(
                _middlewareExchange,
                _messagePuplicherRoute,
                (
                    new Message<DomainModel<PingRequest>>
                    {
                        Header =
                        new MessageHeader { CorrelateId = Guid.Empty, Timestamp = 1010 },
                        Body = new DomainModel<PingRequest>()
                        {
                            Model = new PingRequest() { Name = "ping - pong!" },
                            Name = "me @abdo! domain model"
                        }
                        ,
                        Footer = new MessageFooter { Route = _messagePuplicherRoute }
                    }
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
