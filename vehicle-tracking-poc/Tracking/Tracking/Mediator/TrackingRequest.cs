using BuildingAspects.Services;
using DomainModels.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCacheAdapter;

namespace Tracking.Tracking.Mediator
{
    public class TrackingRequest : IRequest<TrackingModel>
    {
        private readonly ICacheProvider _cache;
        private readonly ControllerContext _controller;
        private readonly IFilter _filter;
        public TrackingRequest(ICacheProvider cache, ControllerContext controller, IFilter filter)
        {
            _cache = cache;
            _controller = controller;
            _filter = filter;
        }
        public ICacheProvider Locator => _cache;
        public ControllerContext Controller => _controller;
        public IFilter Filter => _filter;
    }
}
