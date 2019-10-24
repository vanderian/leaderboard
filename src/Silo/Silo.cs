using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Common;
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
        private static readonly Bootstrap Bootstrap = AppConfigurator.BootstrapApp();

        public static void Main(string[] args)
        {
            _silo = new SiloHostBuilder()
                .UseDashboard()
                .AddMemoryGrainStorageAsDefault()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "LeaderBoardApp";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(LeaderBoardGrain).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureCluster(Bootstrap)
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
            Console.WriteLine($"Silo started on {Bootstrap.HostingEnvironment.EnvironmentName} env");
        }

        private static async Task StopSilo()
        {
            await _silo.StopAsync();
            Console.WriteLine("Silo stopped");
            SiloStopped.Set();
        }
    }
}