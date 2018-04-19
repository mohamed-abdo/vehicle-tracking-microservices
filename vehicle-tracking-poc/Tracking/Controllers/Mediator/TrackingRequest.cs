using BuildingAspects.Services;
using DomainModels.System;
using DomainModels.Business;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCacheAdapter;
using System.Collections.Generic;
using System;
using DomainModels.Business.TrackingDomain;

namespace Tracking.Controllers.Mediator
{
    public class TrackingRequest : IRequest<IEnumerable<DomainModels.Business.TrackingDomain.Tracking>>
    {
        private readonly ICacheProvider _cache;
        private readonly ControllerContext _controller;
        private readonly TrackingFilter _model;
        private readonly IMessageRequest<TrackingFilterModel, IEnumerable<DomainModels.Business.TrackingDomain.Tracking>> _messageQuery;
        private readonly Guid _correlationId;
        private readonly IOperationalUnit _opertationalUnit;
        private readonly MiddlewareConfiguration _middlewareConfiguration;
        public TrackingRequest(
            ControllerContext controller,
            TrackingFilter model,
            ICacheProvider cache,
            IMessageRequest<TrackingFilterModel, IEnumerable<DomainModels.Business.TrackingDomain.Tracking>> messageQuery,
            MiddlewareConfiguration middlewareConfiguration,
            Guid correlationId,
            IOperationalUnit operationalUnit)
        {
            _cache = cache;
            _controller = controller;
            _correlationId = correlationId;
            _model = model;
            _messageQuery = messageQuery;
            _opertationalUnit = operationalUnit;
            _middlewareConfiguration = middlewareConfiguration;
        }
        public ICacheProvider Locator => _cache;
        public ControllerContext Controller => _controller;
        public TrackingFilter Model => _model;
        public IMessageRequest<TrackingFilterModel, IEnumerable<DomainModels.Business.TrackingDomain.Tracking>> MessageQuery => _messageQuery;
        public Guid CorrelationId => _correlationId;
        public IOperationalUnit OperationalUnit => _opertationalUnit;
        public MiddlewareConfiguration MiddlewareConfiguration => _middlewareConfiguration;

    }
}
