using BackgroundMiddleware;
using BuildingAspects.Behaviors;
using BuildingAspects.Services;
using DomainModels.System;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Ping;
using Ping.Contracts;
using Ping.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
namespace PingTests.UnitTest
{
    public class PingTest
    {
        private IPing _ping;
        private string _vehicleId;
        private PingRequest _pingRequest;
        private readonly ILogger<VehicleController> _logger;
        private readonly IMediator _mediatorMock;
        private readonly IServiceLocator _serviceMediatorMock;
        public PingTest()
        {
            var loggerFactortMoq = new Mock<ILoggerFactory>().Object;
            _logger = loggerFactortMoq.CreateLogger<VehicleController>();

            _mediatorMock = new Mock<IMediator>().Object;

            _serviceMediatorMock = new Mock<IServiceLocator>().Object;

            _ping = new VehicleController(_logger, _mediatorMock, _serviceMediatorMock);
            _vehicleId = Guid.NewGuid().ToString();
            _pingRequest = new PingRequest { Status = VehicleStatus.active, Description = "new vehicle!" };
        }

        [Fact]
        public async Task PostPing()
        {
            var result = await _ping.Post(_vehicleId, _pingRequest, new System.Threading.CancellationToken());
            Assert.NotNull(result);
        }
    }
}
