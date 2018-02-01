using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Ping;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace PingTests.IntegrationTest
{
	public class PingTest : IClassFixture<ContainerService>
	{
		private readonly TestServer _server;
		private readonly HttpClient _client;

		public PingTest()
		{
			_server = new TestServer(
				new WebHostBuilder()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddEnvironmentVariables();
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConsole();
					logging.AddDebug();
				})
				.UseStartup<Startup>()
				);

			_client = _server.CreateClient();
		}
		[Fact]
		public async Task GetPing()
		{
			var result = await _client.GetAsync("api/v1/Vehicle/1010");
			result.EnsureSuccessStatusCode();
			Assert.True(true, "ping post failed!");
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
