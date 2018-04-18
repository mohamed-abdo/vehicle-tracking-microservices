using BuildingAspects.Behaviors;
using DomainModels.Types;
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
    public class VehiclePublisherHandler : INotificationHandler<VehiclePublisher>
    {
        private readonly ILogger<VehiclePublisherHandler> _logger;
        public VehiclePublisherHandler(ILogger<VehiclePublisherHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(VehiclePublisher notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Publish notification=> {notification.Model}");
            //add correlation id
            notification.Controller.HttpContext.Request.Headers.Add(Identifiers.CorrelationId, new StringValues(notification.CorrelationId.ToString()));

            var request = new DomainModel<DomainModels.Business.Vehicle>
            {
                Header = new MessageHeader
                {
                    CorrelationId = notification.CorrelationId
                },
                Body = notification.Model,
                Footer = new MessageFooter
                {
                    Sender = DomainModels.System.Identifiers.VeihcleServiceName,
                    FingerPrint = notification.Controller.ActionDescriptor.Id,
                    Environment = notification.OperationalUnit.Environment,
                    Assembly = notification.OperationalUnit.Assembly,
                    Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   {Identifiers.MessagePublisherRoute,  notification.MiddlewareConfiguration.MessagePublisherRoute }
                            }, Defaults.JsonSerializerSettings),
                    Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                }
            };
            await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
            {
                return notification.MessagePublisher.Command(request);
            });
        }
    }
}
