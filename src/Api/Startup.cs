using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var client = CreateClusterClient(services.BuildServiceProvider());
            services.AddGrpc();
            services.AddSingleton(client);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Services.PongApiService>();
                endpoints.MapGet("/",
                    async context => { await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client."); });
            });
        }

        private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
        {
            var log = serviceProvider.GetService<ILogger<Startup>>();
            var count = 0;

            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "LeaderBoardApp";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IPlayer).Assembly))
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ILeaderBoard).Assembly))
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            client.Connect(RetryFilter).GetAwaiter().GetResult();
            log.Info("Client successfully connected to silo host \n");
            return client;

            async Task<bool> RetryFilter(Exception exception)
            {
                log?.LogWarning($"Exception while attempting ({count}) to connect to Orleans cluster: {exception}");
                await Task.Delay(TimeSpan.FromSeconds(5));
                count++;
                return count < 5;
            }
        }
    }
}