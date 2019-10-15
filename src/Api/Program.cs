using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Program
    {
        public static readonly Guid GameId = Guid.NewGuid();

        public static void Main(string[] args)
        {
            var client = Client.ConnectClient().Result;

            CreateHostBuilder(args)
                .ConfigureServices(collection => collection.AddSingleton(client))
                .Build().Run();
        }

        private static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}