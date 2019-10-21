using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grains.Interfaces;
using Grains.Interfaces.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Api.Services
{
    public class PongApiService : Api.PongApiService.PongApiServiceBase
    {
        private readonly ILogger _logger;
        private readonly IClusterClient _client;

        public PongApiService(ILogger<PongApiService> logger, IClusterClient client)
        {
            _logger = logger;
            _client = client;
        }

        public override Task<PlayerScore> AddScore(NewScore request, ServerCallContext context)
        {
            var player = _client.GetGrain<IPlayer>(Guid.Parse(request.Id));
            var leaderBoard = _client.GetGrain<ILeaderBoard>(Program.GameId);
            return leaderBoard.AddPlayerScore(player, request.Score).ContinueWith(task => task.Result.ToPlayerScore());
        }

        public override Task<PlayerScore> GetScore(PlayerId request, ServerCallContext context)
        {
            var player = _client.GetGrain<IPlayer>(Guid.Parse(request.Id));
            var leaderBoard = _client.GetGrain<ILeaderBoard>(Program.GameId);
            return leaderBoard.GetPlayerScore(player).ContinueWith(t => t.Result.ToPlayerScore());
        }

        public override Task<PlayerId> Login(Empty request, ServerCallContext context)
        {
            return Task.FromResult(Guid.NewGuid().ToPlayerId());
        }

        public override Task<PlayerScores> GetScores(Empty request, ServerCallContext context)
        {
            var leaderBoard = _client.GetGrain<ILeaderBoard>(Program.GameId);
            return leaderBoard.GetEntries(0, 10).ContinueWith(t => t.Result.ToPlayerScores());
        }

        public override Task<Empty> SetName(NewPlayerName request, ServerCallContext context)
        {
            var player = _client.GetGrain<IPlayer>(Guid.Parse(request.Id));
            return player.SetPlayerInfo(new PlayerInfo(request.Name)).ContinueWith(t => new Empty());
        }
    }
}