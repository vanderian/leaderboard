using System.Threading.Tasks;
using Grains.Interfaces;
using Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class PlayerGrain : Grain<PlayerState>, IPlayer
    {
        private readonly ILogger _logger;

        public PlayerGrain(ILogger<PlayerGrain> logger)
        {
            _logger = logger;
        }

        public Task SetPlayerInfo(PlayerInfo info)
        {
            State.PlayerInfo = info;
            _logger.LogInformation("Player(@{pkey}) information set {@info}", this.GetPrimaryKey(), info);
            return WriteStateAsync();
        }

        public Task<PlayerInfo> GetPlayerInfo()
        {
            return Task.FromResult(State.PlayerInfo ?? new PlayerInfo(""));
        }
    }

    public class PlayerState
    {
        public PlayerInfo PlayerInfo { get; set; }
    }
}