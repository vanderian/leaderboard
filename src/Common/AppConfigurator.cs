using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Common
{
    public static class AppConfigurator
    {
        public static IConfigurationBuilder AddCommonConfigJson(this IConfigurationBuilder config, IHostEnvironment env)
        {
            return config
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("commonsettings.json", false, true)
                .AddJsonFile("commonsettings." + env.EnvironmentName + ".json", true, true);
        }

        public static Bootstrap BootstrapApp()
        {
            var environment = GetEnvironment();
            var hostingEnvironment = GetHostingEnvironment(environment);
            var configurationBuilder = CreateConfigurationBuilder(hostingEnvironment);

            return new Bootstrap(hostingEnvironment, configurationBuilder);
        }

        private static string GetEnvironment()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return string.IsNullOrEmpty(environmentName) ? "local" : environmentName;
        }

        private static IHostEnvironment GetHostingEnvironment(string environmentName)
        {
            return new HostingEnvironment
            {
                EnvironmentName = environmentName,
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)
            };
        }

        private static IConfigurationBuilder CreateConfigurationBuilder(IHostEnvironment env)
        {
            var config = new ConfigurationBuilder()
                .AddCommonConfigJson(env)
                .AddEnvironmentVariables();

            return config;
        }
    }
}