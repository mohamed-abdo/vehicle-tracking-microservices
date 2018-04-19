using BuildingAspects.Behaviors;
using DomainModels.Business.VehicleDomain;
using DomainModels.Types.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vehicle.Controllers.Mediator
{
    public class VehicleRequestHandler : IRequestHandler<VehicleRequest, IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>>
    {
        private readonly ILogger<VehicleRequestHandler> _logger;
        public VehicleRequestHandler(ILogger<VehicleRequestHandler> logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<DomainModels.Business.VehicleDomain.Vehicle>> Handle(VehicleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Request => {nameof(VehicleRequest)}");
            //add correlation id
            request.Controller.HttpContext.Request.Headers.Add(DomainModels.Types.Identifiers.CorrelationId, new StringValues(request.CorrelationId.ToString()));

            var VehicleFilterModel = new VehicleFilterModel
            {
                Header = new MessageHeader
                {
                    CorrelationId = request.CorrelationId
                },
                Body = request.Model,
                Footer = new MessageFooter
                {
                    Sender = DomainModels.System.Identifiers.TrackingServiceName,
                    FingerPrint = request.Controller.ActionDescriptor.Id,
                    Environment = request.OperationalUnit.Environment,
                    Assembly = request.OperationalUnit.Assembly,
                    Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   { DomainModels.System.Identifiers.MessagePublisherRoute,  request.MiddlewareConfiguration.MessagePublisherRoute }
                             }, Defaults.JsonSerializerSettings),
                    Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                }
            };
            return await await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(async () =>
            {
                return await request.MessageRequest.Request(VehicleFilterModel);
            });
        }
    }
}
