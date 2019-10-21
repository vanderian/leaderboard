using System;
using System.Linq;
using Grains.Interfaces.Models;

namespace Api
{
    public static class Extensions
    {
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