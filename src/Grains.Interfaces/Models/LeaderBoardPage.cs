using System.Collections.Generic;

namespace Grains.Interfaces.Models
{
    public class LeaderBoardPage
    {
        public LeaderBoardPage(int total, int offset, List<LeaderBoardEntry> entries)
        {
            Total = total;
            Offset = offset;
            Entries = entries;
        }

        public int Total { get; set; }
        public int Offset { get; set; }
        public List<LeaderBoardEntry> Entries { get; set; }
    }
}