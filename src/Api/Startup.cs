using System;
using System.Threading.Tasks;
using Common.Config;
using Grains.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;

namespace Api
{
    public class Startup
    {
        private readonly IConfiguration _cfg;

        public Startup(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.Configure<OrleansConfig>(_cfg.GetSection(nameof(OrleansConfig)));
            services.Configure<AwsKeys>(_cfg.GetSection(nameof(AwsKeys)));

//            connect immediately
//            var client = CreateClusterClient(services.BuildServiceProvider());
//            services.AddSingleton(client);

//            connect lazy
            services.AddSingleton(CreateClusterClient);
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
                .ConfigureCluster(serviceProvider)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrainMarker).Assembly).WithReferences())
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