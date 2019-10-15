using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Grains.Tests.Hosted.Cluster;
using Orleans.TestingHost;
using Xunit;

namespace Grains.Tests.Hosted
{
    [Collection(ClusterCollection.Name)]
    public class LeaderBoardGrainTests
    {
        private readonly TestCluster _cluster;

        public LeaderBoardGrainTests(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async Task ShouldGetRank()
        {
            var gameId = Guid.NewGuid();
            IPlayer Player() => _cluster.GrainFactory.GetGrain<IPlayer>(Guid.NewGuid());

            var p1 = Tuple.Create(10, Player());
            var p2 = Tuple.Create(20, Player());
            var p3 = Tuple.Create(30, Player());
            var p4 = Tuple.Create(40, Player());
            var p5 = Tuple.Create(50, Player());
            var leaderBoard = _cluster.GrainFactory.GetGrain<ILeaderBoard>(gameId);

            await Task.WhenAll(
                leaderBoard.AddPlayerScore(p1.Item2, p1.Item1),
                leaderBoard.AddPlayerScore(p2.Item2, p2.Item1),
                leaderBoard.AddPlayerScore(p3.Item2, p3.Item1),
                leaderBoard.AddPlayerScore(p4.Item2, p4.Item1),
                leaderBoard.AddPlayerScore(p5.Item2, p5.Item1)
            );

            var rank = await leaderBoard.GetPlayerScore(p3.Item2);

            Assert.Equal(3, rank.Rank);
        }
    }
}