using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Api
{
    public class Program
    {
        public static readonly Guid GameId = Guid.NewGuid();

        public static void Main(string[] args)
        {
            CreateHost(args).Run();
        }

        private static IWebHost CreateHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}