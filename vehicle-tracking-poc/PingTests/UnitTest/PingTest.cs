using BackgroundMiddleware.Abstract;
using BuildingAspects.Services;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using Moq;
using Ping;
using Ping.Contracts;
using Ping.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace PingTests.UnitTest
{
	public class PingTest
    {
        private IPing _ping;
        private string _vehicleId;
        private PingRequest _pingRequest;
        private readonly ILogger<VehicleController> _logger;
        private readonly IOperationalUnit _operationalUnit;
        private readonly LocalConfiguration _localConfiguration;
        private readonly IMessagePublisher _publisherMock;

        public PingTest()
        {
            var loggerFactortMoq = new Mock<ILoggerFactory>().Object;
            _logger = loggerFactortMoq.CreateLogger<VehicleController>();

            _operationalUnit = new OperationalUnit(
                environment: "IntegrationTest",
                assembly: $"{Environment.MachineName} {this.GetType().Assembly.FullName} V{this.GetType().Assembly.GetName().Version}");

            _localConfiguration = LocalConfiguration.Create(new Dictionary<string, string>() {
                {nameof(_localConfiguration.CacheServer),"distributed_cache"},
                {nameof(_localConfiguration.CacheDBVehicles),  "vehicles"},
                {nameof(_localConfiguration.MessagesMiddleware),  "messages_middleware"},
                {nameof(_localConfiguration.MiddlewareExchange),  "platform"},
                {nameof(_localConfiguration.MessagePublisherRoute),  "*.ping.vehicle"},
                {nameof(_localConfiguration.MessagesMiddlewareUsername),  "guest"},
                {nameof(_localConfiguration.MessagesMiddlewarePassword),  "guest"},
            });

            _publisherMock = new Mock<IMessagePublisher>().Object;

			_ping = new VehicleController(_logger, _publisherMock, _localConfiguration, _operationalUnit);
			_vehicleId = Guid.NewGuid().ToString();
			_pingRequest = new PingRequest { Status = VehicleStatus.active, Description = "new vehicle!" };
		}

        [Fact]
        public void PostPing()
        {
            var result = _ping.Post(_vehicleId, _pingRequest);
            Assert.NotNull(result);
        }
    }
}
