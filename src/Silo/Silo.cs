using System;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Silo
{
    class Program
    {
        private static ISiloHost _silo;
        private static readonly ManualResetEvent SiloStopped = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            _silo = new SiloHostBuilder()
                .UseDashboard()
                .UseLocalhostClustering()
                .AddMemoryGrainStorageAsDefault()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "LeaderBoardApp";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(LeaderBoardGrain).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(StopSilo);
                SiloStopped.WaitOne();
            };

            SiloStopped.WaitOne();
        }

        private static async Task StartSilo()
        {
            await _silo.StartAsync();
            Console.WriteLine("Silo started");
        }

        private static async Task StopSilo()
        {
            await _silo.StopAsync();
            Console.WriteLine("Silo stopped");
            SiloStopped.Set();
        }
    }
}