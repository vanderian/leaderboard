using System;
using System.Net;
using Common;
using Common.Config;
using Microsoft.Extensions.Configuration;
using Orleans.Clustering.Kubernetes;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Silo
{
    public static class Extensions
    {
        public static ISiloHostBuilder ConfigureCluster(this ISiloHostBuilder builder, Bootstrap bootstrap)
        {
            var cfg = bootstrap.Configuration.GetSection(nameof(OrleansConfig)).Get<OrleansConfig>();

            if (!bootstrap.HostingEnvironment.IsValid())
                throw new ArgumentException($"ERROR: ASPNETCORE_ENVIRONMENT set to unknown value: {bootstrap.HostingEnvironment.EnvironmentName}");

            if (bootstrap.HostingEnvironment.IsLocal())
            {
                builder.UseLocalhostClustering()
                    .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
            }

            if (bootstrap.HostingEnvironment.IsK8())
            {
                builder.UseKubeMembership(options => { options.CanCreateResources = true; })
                    .ConfigureEndpoints(new Random(1).Next(10001, 10100), new Random(1).Next(20001, 20100));
            }

            return builder;
        }
    }
}