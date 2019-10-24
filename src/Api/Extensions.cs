using System;
using System.Linq;
using Common;
using Grains.Interfaces.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Clustering.Kubernetes;

namespace Api
{
    public static class Extensions
    {
        public static IWebHostBuilder ApplyCommonConfig(this IWebHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((ctx, cfg) => cfg.AddCommonConfigJson(ctx.HostingEnvironment));
        }

        public static IClientBuilder ConfigureCluster(this IClientBuilder builder, IHostEnvironment env)
        {
            if (env.IsLocal())
            {
                builder.UseLocalhostClustering();
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