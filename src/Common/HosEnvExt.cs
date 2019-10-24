using Microsoft.Extensions.Hosting;

namespace Common
{
    public static class HosEnvExt
    {
        public const string LocalEnvironment = "LOCAL";
        public const string AwsEnvironment = "AWS";
        public const string K8Environment = "K8";

        public static bool IsValid(this IHostEnvironment env)
        {
            return env.IsLocal() || env.IsAws() || env.IsK8();
        }

        public static bool IsLocal(this IHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment(LocalEnvironment);
        }

        public static bool IsAws(this IHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment(AwsEnvironment);
        }

        public static bool IsK8(this IHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment(K8Environment);
        }
    }
}