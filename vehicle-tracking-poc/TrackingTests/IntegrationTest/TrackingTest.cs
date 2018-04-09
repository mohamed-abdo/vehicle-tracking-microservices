using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tracking;
using Xunit;

namespace TrackingTests.IntegrationTest
{
    public class TrackingTest : IClassFixture<ContainerService>
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly string _trackingSrvUri = "/api/v1/Tracking/1010";

        public TrackingTest()
        {
            var hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            _server = new TestServer(hostBuilder);
            //ping client service
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:32771");
        }
        [Fact]
        public async Task GetTracking()
        {
            var result = await _client.GetAsync(_trackingSrvUri);
            result.EnsureSuccessStatusCode();
            Assert.True(true, "tracking get failed!");
        }
    }
}
