using System;
using Orleans.Concurrency;

namespace Grains.Interfaces.Models
{
    [Immutable]
    public class PlayerInfo
    {
        public PlayerInfo(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}