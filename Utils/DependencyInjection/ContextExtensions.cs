using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utils.Core.Options;

namespace Utils.DependencyInjection
{
    public static class ContextExtensions
    {
        public static IServiceCollection CreateContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BuildOptions>(configuration.GetSection("Build"));
            services.Configure<GitOptions>(configuration.GetSection("Git"));
            return services;
        }
    }
}
