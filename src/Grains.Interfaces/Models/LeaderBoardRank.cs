namespace Grains.Interfaces.Models
{
    public class LeaderBoardRank
    {
        public LeaderBoardRank(int score, int rank)
        {
            Score = score;
            Rank = rank;
        }

        public int Score { get; set; }
        public int Rank { get; set; }
    }
}