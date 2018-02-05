using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Ping;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using System;

namespace PingTests.IntegrationTest
{
    public class PingTest : IClassFixture<ContainerService>
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly string _pingSrvUri = "api/v1/Vehicle/1010";
        private readonly IHostingEnvironment Environment;

        public PingTest()
        {
            var hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            _server = new TestServer(hostBuilder);
            //ping client service
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:32777/");
        }
        [Fact]
        public async Task GetPing()
        {
            var result = await _client.GetAsync(_pingSrvUri);

            result.EnsureSuccessStatusCode();
            Assert.True(true, "ping get failed!");
        }

        [Fact]
        public async Task PostPing()
        {
            var result = await _client.GetAsync("api/v1/Vehicle/1010");
            result.EnsureSuccessStatusCode();
            Assert.True(true, "ping post failed!");
        }

    }
}
