using System;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using BuildingAspects.Behaviors;
using DomainModels.System;
using DomainModels.Types;
using DomainModels.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebComponents.Interceptors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ping
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        // GET api/v/<controller>/5
        [CustomHeader("author", "mohamed-abdo=>mohamad.abdo@gmail.com")]
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            // message definition
            //(MessageHeader Header, DomainModel<PingRequest> Body, MessageFooter Footer)
            Task.Run(() =>
            {
                new Function(_logger, Identifiers.RetryCount).Decorate(() =>
                  {
                      return _publisher.Publish(
                          _localConfiguration.MiddlewareExchange,
                          _localConfiguration.MessagePublisherRoute,
                          (
                              Header: new MessageHeader { CorrelateId = Guid.Empty },
                              Body: new DomainModel<PingModel>()
                              {
                                  Model = new PingModel() { ChassisNumber = Guid.NewGuid(), Message = "ping - pong!" }
                              },
                              Footer: new MessageFooter { Route = _localConfiguration.MessagePublisherRoute }
                          ));
                  });
            });
            //return NotFound();
            return Content(id);
        }

        // POST api/v/<controller>
        [HttpPost("{id}")]
        public void Post([FromBody]string value)
        {

        }
    }
}
