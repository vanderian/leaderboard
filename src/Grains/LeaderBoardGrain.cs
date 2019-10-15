using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.Interfaces;
using Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class LeaderBoardGrain : Grain, ILeaderBoard
    {
        private readonly ILogger _logger;

        private readonly List<LeaderBoardEntry> _entries = new List<LeaderBoardEntry>();

        public LeaderBoardGrain(ILogger<LeaderBoardGrain> logger)
        {
            _logger = logger;
        }

        public Task<LeaderBoardRank> GetPlayerScore(IPlayer player)
        {
            var idx = PlayerIndex(player);
            return Task.FromResult(new LeaderBoardRank(_entries[idx].Score, idx + 1));
        }

        public async Task<LeaderBoardRank> AddPlayerScore(IPlayer player, int score)
        {
            var idxPlayer = PlayerIndex(player);
            var idxInsert = Math.Max(0, _entries.FindIndex(e => e.Score < score));
            if (idxPlayer < 0)
            {
                var info = await player.GetPlayerInfo();
                _entries.Insert(idxInsert, new LeaderBoardEntry(player.GetPrimaryKey(), info, score));
                _logger.LogInformation("Adding new score: {score} for player {info.Name}");
            }
//            update score if higher
            else if (_entries[idxPlayer].Score <= score)
            {
                var info = _entries[idxPlayer].PlayerInfo;
                _entries.RemoveAt(idxPlayer);
                _entries.Insert(idxInsert, new LeaderBoardEntry(player.GetPrimaryKey(), info, score));
                _logger.LogInformation("Update score: {score} for player {info.Name}");
            }

            return new LeaderBoardRank(score, idxInsert + 1);
        }

        public Task<LeaderBoardPage> GetEntries(int offset, int count)
        {
            var range = _entries.GetRange(offset, count);
            return Task.FromResult(new LeaderBoardPage(_entries.Count, offset, range));
        }

        private int PlayerIndex(IPlayer player)
        {
            return _entries.FindIndex(x => x.PlayerId == player.GetPrimaryKey());
        }
    }
}