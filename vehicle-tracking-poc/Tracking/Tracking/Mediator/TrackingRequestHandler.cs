using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tracking.Tracking.Mediator
{
    public class TrackingRequestHandler : IRequestHandler<TrackingRequest, IEnumerable<(MessageHeader, PingModel, MessageFooter)>>
    {
        private readonly ILogger<TrackingRequestHandler> _logger;
        public TrackingRequestHandler(ILogger<TrackingRequestHandler> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<(MessageHeader, PingModel, MessageFooter)>> Handle(TrackingRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request => {nameof(TrackingModel)}");

            return await await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
             {
                 return request.MessageQuery.Query(
                     (
                         Header: new MessageHeader
                         {
                             CorrelationId = request.OperationalUnit.InstanceId
                         },
                         Body: request.Model
                         ,
                         Footer: new MessageFooter
                         {
                             Sender = request.Controller.ActionDescriptor.DisplayName,
                             FingerPrint = request.Controller.ActionDescriptor.Id,
                             Environment = request.OperationalUnit.Environment,
                             Assembly = request.OperationalUnit.Assembly,
                             Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   { DomainModels.Types.Identifiers.MessagePublisherRoute,  request.MiddlewareConfiguration.MessagePublisherRoute }
                             }, Defaults.JsonSerializerSettings),
                             Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                         }
                     ));

             });
        }
    }
}
