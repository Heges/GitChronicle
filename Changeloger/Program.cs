using Changeloger.Models;
using Changeloger.Services;
using ChangelogParser.Models;
using Changloger.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Utils.Core.Interfaces;
using Utils.DependencyInjection;
using Utils.Utils;

namespace Changloger;

class Program
{
    private static IServiceProvider _serviceProvider;

    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("Logs/log_.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        try
        {
            Log.Information("Приложение запускается...");

            ServiceInitializing(args);
            ApplicationInitializing();

            Run(args).GetAwaiter().GetResult();

            Console.WriteLine("Приложение завершено, можно закрыть это окно.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение завершилось с фатальной ошибкой.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ServiceInitializing(string[] args = default)
    {
        Log.Information("Инициализация сервисов...");

        _serviceProvider = ConfigureServices(args);

        Log.Information("Сервисы проиницилизированны.");
    }

    private static IServiceProvider ConfigureServices(string[] args = default)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .AddInMemoryCollection(new ArgumentParser(args)
                .RegisterArgsToConfiguration()
            )
            .Build();

        var services = new ServiceCollection();
        
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddUtills()
            .CreateContext(configuration);
        services.AddOptions<PDFOptions>();
        services.AddTransient<Changelog>();
        services.AddTransient<ChangelogItem>();
        services.AddSingleton<LoadChangelog>();
        services.AddSingleton<CreatePDFDocument>();

        return services.BuildServiceProvider();
    }

    private static void ApplicationInitializing()
    {
        Log.Information("Инициализация приложения...");

        var appsettings = _serviceProvider.GetRequiredService<IConfiguration>();

        Log.Information("Приложение проиницилизированно.");
    }

    private static async Task Run(string[] args)
    {
        Console.WriteLine("-".FillingWidthWithSymbol());
        Console.WriteLine("CTRL + C для выхода из программы");
        Console.WriteLine("-".FillingWidthWithSymbol());

        var utils = _serviceProvider.GetRequiredService<Pipeline>();
        await utils.Run();

        var changelog =  _serviceProvider.GetRequiredService<LoadChangelog>();
        var current_state = _serviceProvider.GetRequiredService<IContext>();
        var changelogs = changelog.Load(current_state.BuildOptions.RootFolder, current_state.BuildOptions.Platform);
        var pdfdocument = _serviceProvider.GetRequiredService<CreatePDFDocument>();
        var document = pdfdocument.Create(current_state.BuildOptions.RootFolder, changelogs, current_state.BuildOptions.Platform);
    }

}