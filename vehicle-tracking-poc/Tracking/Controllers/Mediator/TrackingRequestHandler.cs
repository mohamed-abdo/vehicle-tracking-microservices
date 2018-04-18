using BuildingAspects.Behaviors;
using DomainModels.Types;
using DomainModels.Types.Messages;
using DomainModels.Business;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Tracking.Controllers.Mediator
{
    public class TrackingRequestHandler : IRequestHandler<TrackingRequest, IEnumerable<DomainModels.Business.Tracking>>
    {
        private readonly ILogger<TrackingRequestHandler> _logger;
        public TrackingRequestHandler(ILogger<TrackingRequestHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<DomainModels.Business.Tracking>> Handle(TrackingRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request => {nameof(TrackingModel)}");
            //add correlation id
            request.Controller.HttpContext.Request.Headers.Add(Identifiers.CorrelationId, new StringValues(request.CorrelationId.ToString()));

        
            var trackingFilterModel = new TrackingFilterModel
            {
                Header = new MessageHeader
                {
                    CorrelationId = request.OperationalUnit.InstanceId
                },
                Body = request.Model,
                Footer = new MessageFooter
                {
                    Sender = DomainModels.System.Identifiers.TrackingServiceName,
                    FingerPrint = request.Controller.ActionDescriptor.Id,
                    Environment = request.OperationalUnit.Environment,
                    Assembly = request.OperationalUnit.Assembly,
                    Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   { Identifiers.MessagePublisherRoute,  request.MiddlewareConfiguration.MessagePublisherRoute }
                             }, Defaults.JsonSerializerSettings),
                    Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                }
            };
            return await await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
             {
                 return request.MessageQuery.Query(trackingFilterModel);
             });
        }
    }
}
