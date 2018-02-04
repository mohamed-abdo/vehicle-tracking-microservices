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
		private readonly Uri _pingSrvUri;
		private readonly IHostingEnvironment Environment;

		public PingTest()
		{
			IHostingEnvironment hostingEnvironment = null;
			_server = new TestServer(
				new WebHostBuilder()
				.UseKestrel()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					hostingEnvironment = hostingContext.HostingEnvironment;
					config.AddEnvironmentVariables();
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConsole();
					logging.AddDebug();
				})
				.UseStartup<Startup>()
				);
			Environment = hostingEnvironment;
			_client = _server.CreateClient();
			_pingSrvUri = new Uri("http://localhost:32777/api/v1/Vehicle/1010");
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
