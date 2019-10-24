using System.Threading.Tasks;
using Grains.Interfaces.Models;
using Orleans;

namespace Grains.Interfaces
{
    public interface ILeaderBoard : IGrainWithGuidKey, IGrainMarker
    {
        Task<LeaderBoardRank> GetPlayerScore(IPlayer player);
        Task<LeaderBoardRank> AddPlayerScore(IPlayer player, int score);
        Task<LeaderBoardPage> GetEntries(int offset, int count);
    }
}