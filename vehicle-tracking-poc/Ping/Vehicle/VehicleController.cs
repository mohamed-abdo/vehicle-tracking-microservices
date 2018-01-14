using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Types;
using DomainModels.Types.Exceptions;
using DomainModels.Types.Messages;
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
        private readonly IOperationalUnit _operationalUnit;
        private readonly IMessagePublisher _publisher;
        private readonly LocalConfiguration _localConfiguration;
        public VehicleController(
            ILogger<VehicleController> logger,
            IMessagePublisher publisher,
            LocalConfiguration localConfiguration,
            IOperationalUnit operationalUnit)
        {
            _logger = logger;
            _operationalUnit = operationalUnit;
            _publisher = publisher;
            _localConfiguration = localConfiguration;
        }

        // GET api/v/<controller>/5
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.PingDomainModel)]
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            //TODO:in the future in case of correlated action, link them by correlation header, ....       
            //[CustomHeader(Models.Identifiers.CorrelationId, _operationalUnit.OperationId)]

            // message definition
            //(MessageHeader Header, DomainModel<PingRequest> Body, MessageFooter Footer)
            Task.Run(() =>
            {
                new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
                  {
                      return _publisher.Publish(
                          _localConfiguration.MiddlewareExchange,
                          _localConfiguration.MessagePublisherRoute,
                          (
                              Header: new MessageHeader
                              {
                                  CorrelateId = _operationalUnit.InstanceId
                              },
                              Body: new DomainModel<PingModel>()
                              {
                                  Model = new PingModel() { ChassisNumber = Guid.NewGuid(), Message = "ping - pong!" }
                              },
                              Footer: new MessageFooter
                              {
                                  Sender = ControllerContext.ActionDescriptor.DisplayName,
                                  Environment = _operationalUnit.Environment,
                                  Assembly = _operationalUnit.Assembly,
                                  FingerPrint = ControllerContext.ActionDescriptor.Id,
                                  Route = new Dictionary<string, string> { { DomainModels.Types.Identifiers.MessagePublisherRoute, _localConfiguration.MessagePublisherRoute } },
                                  Hint = ResponseHint.OK
                              }
                          ));
                  });
            });
            //throw new CustomException(code: ExceptionCodes.MessageMalformed);
            return Content(id);
        }
    }
}
