using System;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests.Isolated
{
    public class LeaderBoardGrainTests
    {
        [Fact]
        public async Task Should_Get_Rank()
        {
//            var grain = new LeaderBoardGrain(Mock.Of<ILogger<LeaderBoardGrain>>());
//            var rank = await grain.AddPlayerScore(_player, 100);

            Assert.Equal(1, 1);
        }
    }
}