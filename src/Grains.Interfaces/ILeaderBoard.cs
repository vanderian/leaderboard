using System.Threading.Tasks;
using Grains.Interfaces.Models;
using Orleans;

namespace Grains.Interfaces
{
    public interface ILeaderBoard : IGrainWithGuidKey
    {
        Task<int> GetRank(IPlayer player);
        Task AddPlayerScore(IPlayer player, int score);
        Task<LeaderBoardPage> GetEntries(int offset, int count);
    }
}