using DomainModels.Vehicle;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tracking.Tracking.Mediator
{
    public class TrackingRequestHandler : IRequestHandler<TrackingRequest, TrackingModel>
    {
        private readonly ILogger<TrackingRequestHandler> _logger;
        public TrackingRequestHandler(ILogger<TrackingRequestHandler> logger)
        {
            _logger = logger;
        }


        public Task<TrackingModel> Handle(TrackingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
