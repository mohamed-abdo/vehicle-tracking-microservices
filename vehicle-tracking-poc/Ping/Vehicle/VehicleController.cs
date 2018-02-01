using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackgroundMiddleware.Abstract;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ping.Contracts;
using Ping.Models;
using WebComponents.Interceptors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ping
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VehicleController : Controller, IPing
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
        public async Task<IActionResult> Get(string id)
        {
            //TODO:in the future in case of correlated action, link them by correlation header, ....       
            //[CustomHeader(Models.Identifiers.CorrelationId, _operationalUnit.OperationId)]

            // message definition
            //(MessageHeader Header,PingModel Body, MessageFooter Footer)

            await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
               {
                   return _publisher.Publish(
                       _localConfiguration.MiddlewareExchange,
                       _localConfiguration.MessagePublisherRoute,
                       (
                           Header: new MessageHeader
                           {
                               CorrelateId = _operationalUnit.InstanceId
                           },
                           Body: new PingModel() { Status = StatusModel.Active, ChassisNumber = "vehicle@123Xyz!", Message = "ping - pong!" }
                           ,
                           Footer: new MessageFooter
                           {
                               Sender = ControllerContext.ActionDescriptor.DisplayName,
                               Environment = _operationalUnit.Environment,
                               Assembly = _operationalUnit.Assembly,
                               FingerPrint = ControllerContext.ActionDescriptor.Id,
                               Route = new Dictionary<string, string> {
                                   { DomainModels.Types.Identifiers.MessagePublisherRoute,  _localConfiguration.MessagePublisherRoute }
                               },
                               Hint = ResponseHint.OK
                           }
                       ));

               });
            //throw new CustomException(code: ExceptionCodes.MessageMalformed);
            return Content(id);
        }

        // POST api/v/<controller>/vehicleId
        [CustomHeader(Models.Identifiers.DomainModel, Models.Identifiers.PingDomainModel)]
        [HttpPost("{vehicleId}")]
        public IAsyncResult Post(string vehicleId, PingRequest pingRequest)
        {
            throw new NotImplementedException();
        }
    }
}
