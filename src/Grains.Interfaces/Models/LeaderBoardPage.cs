using System.Collections.Generic;

namespace Grains.Interfaces.Models
{
    public class LeaderBoardPage
    {
        public LeaderBoardPage(int total, int offset, List<LeaderBoardRank> ranks)
        {
            Total = total;
            Offset = offset;
            Ranks = ranks;
        }

        public int Total { get; set; }
        public int Offset { get; set; }
        public List<LeaderBoardRank> Ranks { get; set; }
    }
}