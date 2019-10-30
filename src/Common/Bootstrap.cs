using Common.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common
{
    public class Bootstrap
    {
        public IHostEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Bootstrap(
            IHostEnvironment hostingEnvironment,
            IConfigurationBuilder configurationBuilder
        )
        {
            HostingEnvironment = hostingEnvironment;
            Configuration = configurationBuilder.Build();
        }

        public T GetConfig<T>()
        {
            return Configuration.GetSection(nameof(T)).Get<T>();
        }
    }
}