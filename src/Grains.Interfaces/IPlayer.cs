using System.Threading.Tasks;
using Grains.Interfaces.Models;
using Orleans;

namespace Grains.Interfaces
{
    public interface IPlayer : IGrainWithGuidKey
    {
        Task SetPlayerInfo(PlayerInfo info);
        Task<PlayerInfo> GetPlayerInfo();
    }
}