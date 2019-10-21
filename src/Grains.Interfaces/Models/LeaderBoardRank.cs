using System;

namespace Grains.Interfaces.Models
{
    public class LeaderBoardRank
    {
        public LeaderBoardRank(int score, int rank, PlayerInfo info, Guid id)
        {
            Score = score;
            Rank = rank;
            PlayerInfo = info;
            PlayerId = id;
        }

        public int Score { get; set; }
        public int Rank { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
        public Guid PlayerId { get; set; }
    }
}