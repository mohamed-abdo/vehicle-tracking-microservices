using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PingTests.IntegrationTest
{
	public class ContainerService : IDisposable
	{
		// Name of the service
		private const string ServiceName = "ping";

		// Relative path to the root folder of the service project.
		// The path is relative to the target folder for the test DLL,
		// i.e. /test/bin/Debug
		private const string ServicePath = "../../../";

		// Tag used for ${TAG} in docker-compose.yml
		private const string Tag = "test";

		// How long to wait for the test URL to return 200 before giving up
		private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(60);

#if DEBUG
		private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

		public ContainerService()
		{
			Build();
			StartContainers();

			void Build()
			{
				var process = Process.Start(new ProcessStartInfo
				{
					FileName = "dotnet",
					Arguments = $"publish {ServicePath} --configuration {Configuration}"
				});

				process.WaitForExit();
				Assert.Equal(0, process.ExitCode);
			}


			void StartContainers()
			{
				// First build the Docker container image
				var processStartInfo = new ProcessStartInfo
				{
					FileName = "docker-compose",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					Arguments = $"-f {ServicePath}../docker-compose.yml build"
				};

				AddEnvironmentVariables(processStartInfo);

				var process = Process.Start(processStartInfo);

				process.WaitForExit();
				Assert.Equal(0, process.ExitCode);

				// Now start the docker containers

				processStartInfo = new ProcessStartInfo
				{
					FileName = "docker-compose",
					Arguments =
						$"-f {ServicePath}../docker-compose.yml -f {ServicePath}../docker-compose.yml -p {ServiceName} up -d"
				};

				AddEnvironmentVariables(processStartInfo);

				process = Process.Start(processStartInfo);

				process.WaitForExit();
				Assert.Equal(0, process.ExitCode);
			}
		}

		private void AddEnvironmentVariables(ProcessStartInfo processStartInfo)
		{
			processStartInfo.Environment["TAG"] = Tag;
			processStartInfo.Environment["CONFIGURATION"] = Configuration;
			processStartInfo.Environment["COMPUTERNAME"] = Environment.MachineName;
		}

		public void Dispose()
		{
			StopContainers();

			void StopContainers()
			{
				// Run docker-compose down to stop the containers
				// Note that "--rmi local" deletes the images as well to keep the machine clean
				// But it does so by deleting all untagged images, which may not be desired in all cases

				var processStartInfo = new ProcessStartInfo
				{
					FileName = "docker-compose",
					Arguments =
						$"-f {ServicePath}../docker-compose.yml -f {ServicePath}../docker-compose.yml -p {ServiceName} down --rmi local"
				};

				AddEnvironmentVariables(processStartInfo);

				var process = Process.Start(processStartInfo);

				process.WaitForExit();
				Assert.Equal(0, process.ExitCode);
			}
		}
	}
}