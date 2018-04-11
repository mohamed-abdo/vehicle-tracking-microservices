using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCacheAdapter;
using System.Collections.Generic;
using System.Linq;

namespace Tracking.Tracking.Mediator
{
    public class TrackingRequest : IRequest<IEnumerable<(MessageHeader, PingModel, MessageFooter)>>
    {
        private readonly ICacheProvider _cache;
        private readonly ControllerContext _controller;
        private readonly TrackingModel _model;
        private readonly IMessageQuery<TrackingModel, IEnumerable<(MessageHeader, PingModel, MessageFooter)>> _messageQuery;
        private readonly IOperationalUnit _oprtationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public TrackingRequest(
            ControllerContext controller,
            TrackingModel model,
            ICacheProvider cache,
            IMessageQuery<TrackingModel, IEnumerable<(MessageHeader, PingModel, MessageFooter)>> messageQuery,
            IOperationalUnit oprtationalUnit,
            MiddlewareConfiguration middlewareConfiguration
            )
        {
            _cache = cache;
            _controller = controller;
            _model = model;
            _messageQuery = messageQuery;
            _oprtationalUnit = oprtationalUnit;
            _middlewareConfiguration = middlewareConfiguration;
        }
        public ICacheProvider Locator => _cache;
        public ControllerContext Controller => _controller;
        public TrackingModel Model => _model;
        public IMessageQuery<TrackingModel, IEnumerable<(MessageHeader, PingModel, MessageFooter)>> MessageQuery => _messageQuery;
        public IOperationalUnit OperationalUnit => _oprtationalUnit;
        public MiddlewareConfiguration MiddlewareConfiguration => _middlewareConfiguration;

    }
}
