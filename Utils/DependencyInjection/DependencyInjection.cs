using Changloger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Services;

namespace Utils.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtills(this IServiceCollection services)
        {
            services.AddSingleton<IState, State>();
            services.AddTransient<IShell, Shell>();
            services.AddTransient<UtilsEntryPoint>();
            services.AddTransient<InitProcess>();
            services.AddTransient<CleanProcess>();
            services.AddTransient<UpdateProcess>();
            services.AddTransient<ChangelogProcess>();

            services.AddTransient<Func<string, IProcess>>(provider => (string_key) =>
            {
                return string_key switch
                {
                    "init" => provider.GetRequiredService<InitProcess>(),
                    "update" => provider.GetRequiredService<UpdateProcess>(),
                    "clean" => provider.GetRequiredService<CleanProcess>(),
                    "changelog" => provider.GetRequiredService<ChangelogProcess>(),
                    _ => throw new InvalidOperationException($"Не поддерживаемый тег ключа скрипта ОС: {string_key}")
                };
            });

            services.AddSingleton<IShell, Shell>();

            return services;
        }
    }
}
