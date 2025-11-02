using Changloger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Core.Options;
using Utils.Services;

namespace Utils.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtills(this IServiceCollection services)
        {
            services.AddOptions<BuildOptions>();
            services.AddOptions<GitOptions>();
            services.AddScoped<IContext, Context>();
            services.AddSingleton<Pipeline>();
            services.AddSingleton<IShell, Shell>();
            services.AddTransient<GitInit>();
            services.AddTransient<Clean>();
            services.AddTransient<GitUpdate>();
            services.AddTransient<GitLog>();

            return services;
        }
    }
}
