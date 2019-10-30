using System;
using System.Linq;
using Common;
using Common.Config;
using Grains.Interfaces.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Clustering.Kubernetes;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Api
{
    public static class Extensions
    {
        public static IWebHostBuilder ApplyCommonConfig(this IWebHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((ctx, cfg) => cfg.AddCommonConfigJson(ctx.HostingEnvironment));
        }

        public static IClientBuilder ConfigureCluster(this IClientBuilder builder, IServiceProvider serviceProvider)
        {
            var env = serviceProvider.GetService<IHostEnvironment>();
            var keys = serviceProvider.GetService<IOptions<AwsKeys>>().Value;
            var cfg = serviceProvider.GetService<IOptions<OrleansConfig>>().Value;

            builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = cfg.ClusterId;
                options.ServiceId = cfg.ServiceId;
            });

            if (env.IsLocal())
            {
                builder.UseLocalhostClustering();
            }

            if (env.IsAws())
            {
                builder.UseDynamoDBClustering(options =>
                {
                    options.AccessKey = keys.ApiKey;
                    options.SecretKey = keys.ApiSecret;
                    options.Service = "eu-central-1";
                    options.TableName = cfg.ServiceId;
                });
            }

            if (env.IsK8())
            {
                builder.UseKubeGatewayListProvider();
            }

            return builder;
        }

        public static Guid Guid(this string id)
        {
            return System.Guid.Parse(id);
        }

        public static PlayerId ToPlayerId(this Guid id)
        {
            return new PlayerId() {Id = id.ToString()};
        }

        public static PlayerScore ToPlayerScore(this LeaderBoardRank rank)
        {
            return new PlayerScore() {Rank = rank.Rank, Score = rank.Score, Name = rank.PlayerInfo.Name, Id = rank.PlayerId.ToString()};
        }

        public static PlayerScores ToPlayerScores(this LeaderBoardPage page)
        {
            return new PlayerScores() {Scores = {page.Ranks.Select(r => r.ToPlayerScore())}};
        }
    }
}