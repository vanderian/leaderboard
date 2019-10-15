using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Api.Services
{
    public class LeaderBoardService : LeaderBoard.LeaderBoardBase
    {
        private readonly ILogger _logger;
        private readonly IClusterClient _client;

        public LeaderBoardService(ILogger<LeaderBoardService> logger, IClusterClient client)
        {
            _logger = logger;
            _client = client;
        }

        public override Task<PlayerRank> AddScore(PlayerScore request, ServerCallContext context)
        {
            var player = _client.GetGrain<IPlayer>(Guid.Parse(request.Id));
            var leaderBoard = _client.GetGrain<ILeaderBoard>(Program.GameId);
            return leaderBoard.AddPlayerScore(player, request.Score).ContinueWith(task => task.Result.toPlayerRank());
        }

        public override Task<PlayerRank> GetRank(PlayerId request, ServerCallContext context)
        {
            var player = _client.GetGrain<IPlayer>(Guid.Parse(request.Id));
            var leaderBoard = _client.GetGrain<ILeaderBoard>(Program.GameId);
            return leaderBoard.GetPlayerScore(player).ContinueWith(t => t.Result.toPlayerRank());
        }
    }
}