using System;
using System.Collections.Generic;

namespace Grains.Interfaces.Models
{
    public class LeaderBoardEntry
    {
        public LeaderBoardEntry(Guid playerId, PlayerInfo playerInfo, int score)
        {
            PlayerId = playerId;
            PlayerInfo = playerInfo;
            Score = score;
            DateTime = DateTime.Now;
        }

        public Guid PlayerId { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
        public int Score { get; set; }
        public DateTime DateTime { get; set; }

        public static IComparer<LeaderBoardEntry> ScoreComparer = Comparer<LeaderBoardEntry>.Create((entry, other) =>
        {
            if (entry.PlayerId == other.PlayerId)
            {
                return 0;
            }

            return entry.Score == other.Score ? 1 : entry.Score.CompareTo(other.Score);
        });
    }
}