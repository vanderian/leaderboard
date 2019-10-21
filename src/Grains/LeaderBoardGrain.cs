using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<LeaderBoardRank> GetPlayerScore(IPlayer player)
        {
            var idx = PlayerIndex(player);
            if (idx != -1) return new LeaderBoardRank(_entries[idx].Score, idx + 1, _entries[idx].PlayerInfo, _entries[idx].PlayerId);

            var info = await player.GetPlayerInfo();
            return new LeaderBoardRank(0, _entries.Count + 1, info, player.GetPrimaryKey());
        }

        public async Task<LeaderBoardRank> AddPlayerScore(IPlayer player, int score)
        {
            LeaderBoardRank rank;
            var idxPlayer = PlayerIndex(player);
            var idxInsert = ScoreIndex(score);
            if (idxPlayer < 0)
            {
                var info = await player.GetPlayerInfo();
                _entries.Insert(idxInsert, new LeaderBoardEntry(player.GetPrimaryKey(), info, score));
                rank = new LeaderBoardRank(score, idxInsert + 1, info, player.GetPrimaryKey());
                _logger.LogInformation($"Adding new score to {idxInsert}: {score} for player {info.Name}");
            }
//            update score if higher
            else if (_entries[idxPlayer].Score <= score)
            {
                var e = _entries[idxPlayer];
                _entries.RemoveAt(idxPlayer);
                _entries.Insert(idxInsert, new LeaderBoardEntry(player.GetPrimaryKey(), e.PlayerInfo, score));
                rank = new LeaderBoardRank(score, idxInsert + 1, e.PlayerInfo, e.PlayerId);
                _logger.LogInformation("Update score: {score} for player {info.Name}");
            }
            else
            {
                var e = _entries[idxPlayer];
                rank = new LeaderBoardRank(e.Score, idxPlayer + 1, e.PlayerInfo, e.PlayerId);
            }

            return rank;
        }

        public Task<LeaderBoardPage> GetEntries(int offset, int count)
        {
            var c = Math.Min(count, _entries.Count);
            var range = _entries.GetRange(offset, c)
                .Select((e, idx) => new LeaderBoardRank(e.Score, idx + 1, e.PlayerInfo, e.PlayerId))
                .ToList();
            return Task.FromResult(new LeaderBoardPage(_entries.Count, offset, range));
        }

        private int PlayerIndex(IPlayer player)
        {
            return _entries.FindIndex(x => x.PlayerId == player.GetPrimaryKey());
        }

        private int ScoreIndex(int score)
        {
            var i = _entries.FindIndex(e => e.Score < score);
            return i < 0 ? _entries.Count : i;
        }
    }
}