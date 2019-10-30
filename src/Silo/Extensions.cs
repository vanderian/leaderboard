using System;
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
        public static ISiloHostBuilder ConfigureSilo(this ISiloHostBuilder builder, Bootstrap bootstrap)
        {
            var cfg = bootstrap.Configuration.GetSection(nameof(OrleansConfig)).Get<OrleansConfig>();
            var keys = bootstrap.Configuration.GetSection(nameof(AwsKeys)).Get<AwsKeys>();

            if (!bootstrap.HostingEnvironment.IsValid())
                throw new ArgumentException($"ERROR: ASPNETCORE_ENVIRONMENT set to unknown value: {bootstrap.HostingEnvironment.EnvironmentName}");

            builder
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = cfg.ClusterId;
                    options.ServiceId = cfg.ServiceId;
                });

            if (bootstrap.HostingEnvironment.IsLocal())
            {
                builder
                    .UseLocalhostClustering()
                    .AddMemoryGrainStorageAsDefault();
            }

            if (bootstrap.HostingEnvironment.IsAws())
            {
                builder
                    .UseDynamoDBClustering(options =>
                    {
                        options.AccessKey = keys.ApiKey;
                        options.SecretKey = keys.ApiSecret;
                        options.Service = "us-east-2";
                        options.TableName = cfg.ServiceId;
                    })
                    .ConfigureEndpoints(new Random(1).Next(10001, 10100), new Random(1).Next(20001, 20100))
                    .AddDynamoDBGrainStorageAsDefault(options =>
                        {
                            options.UseJson = true;
                            options.AccessKey = keys.ApiKey;
                            options.SecretKey = keys.ApiSecret;
                            options.Service = "us-east-2";
                            options.TableName = $"{cfg.ServiceId}_{cfg.ClusterId}_GrainState";
                        }
                    );
            }

            if (bootstrap.HostingEnvironment.IsK8())
            {
                builder
                    .AddDynamoDBGrainStorageAsDefault(options =>
                        {
                            options.UseJson = true;
                            options.AccessKey = keys.ApiKey;
                            options.SecretKey = keys.ApiSecret;
                            options.Service = "us-east-2";
                            options.TableName = $"{cfg.ServiceId}_{cfg.ClusterId}_GrainState";
                        }
                    )
                    .UseKubeMembership(options => { options.CanCreateResources = true; })
                    .ConfigureEndpoints(new Random(1).Next(10001, 10100), new Random(1).Next(20001, 20100));
            }

            return builder;
        }
    }
}