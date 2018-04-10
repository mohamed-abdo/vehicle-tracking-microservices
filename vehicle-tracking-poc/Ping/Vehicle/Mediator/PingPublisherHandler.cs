﻿using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ping.Vehicle.Mediator
{
    public class PingPublisherHandler : INotificationHandler<PingPublisher>
    {
        private readonly ILogger<PingPublisherHandler> _logger;
        public PingPublisherHandler(ILogger<PingPublisherHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(PingPublisher notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Publish notification=> {notification.Model}");

            await new Function(_logger, DomainModels.System.Identifiers.RetryCount).Decorate(() =>
            {
                return notification.MessagePublisher.Command(
                    notification.MiddlewareConfiguration.MiddlewareExchange,
                    notification.MiddlewareConfiguration.MessagePublisherRoute,
                    (
                        Header: new MessageHeader
                        {
                            CorrelationId = notification.OperationalUnit.InstanceId
                        },
                        Body: notification.Model
                        ,
                        Footer: new MessageFooter
                        {
                            Sender = notification.Controller.ActionDescriptor.DisplayName,
                            FingerPrint = notification.Controller.ActionDescriptor.Id,
                            Environment = notification.OperationalUnit.Environment,
                            Assembly = notification.OperationalUnit.Assembly,
                            Route = JsonConvert.SerializeObject(new Dictionary<string, string> {
                                   { DomainModels.Types.Identifiers.MessagePublisherRoute,  notification.MiddlewareConfiguration.MessagePublisherRoute }
                            }, Defaults.JsonSerializerSettings),
                            Hint = Enum.GetName(typeof(ResponseHint), ResponseHint.OK)
                        }
                    ));

            });
        }
    }
}
