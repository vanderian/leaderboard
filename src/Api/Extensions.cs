using System;
using Grains.Interfaces.Models;

namespace Api
{
    public static class Extensions
    {
        public static Guid Guid(this string id)
        {
            return System.Guid.Parse(id);
        }

        public static PlayerRank toPlayerRank(this LeaderBoardRank rank)
        {
            return new PlayerRank() {Rank = rank.Score, Score = rank.Score};
        }
    }
}