using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Business;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCacheAdapter;
using System.Collections.Generic;

namespace Tracking.Controllers.Mediator
{
    public class TrackingRequest : IRequest<IEnumerable<PingModel>>
    {
        private readonly ICacheProvider _cache;
        private readonly ControllerContext _controller;
        private readonly TrackingFilter _model;
        private readonly IMessageQuery<TrackingFilterModel, IEnumerable<PingModel>> _messageQuery;
        private readonly IOperationalUnit _oprtationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public TrackingRequest(
            ControllerContext controller,
            TrackingFilter model,
            ICacheProvider cache,
            IMessageQuery<TrackingFilterModel, IEnumerable<PingModel>> messageQuery,
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
        public TrackingFilter Model => _model;
        public IMessageQuery<TrackingFilterModel, IEnumerable<PingModel>> MessageQuery => _messageQuery;
        public IOperationalUnit OperationalUnit => _oprtationalUnit;
        public MiddlewareConfiguration MiddlewareConfiguration => _middlewareConfiguration;

    }
}
