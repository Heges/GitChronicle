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

            ServiceInitializing();
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

    private static void ServiceInitializing()
    {
        Log.Information("Инициализация сервисов...");

        _serviceProvider = ConfigureServices();

        Log.Information("Сервисы проиницилизированны.");
    }

    private static IServiceProvider ConfigureServices()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        
        services.AddUtills();


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
        ConfigurationArgs(args);
        Console.WriteLine("-".FillingWidthWithSymbol());

        var utils = _serviceProvider.GetRequiredService<UtilsEntryPoint>();
        await utils.Run();
    }

    private static void ConfigurationArgs(string[] args)
    {
        var state = _serviceProvider.GetRequiredService<IState>();

        state.ArgumentParser = new ArgumentParser(args);

        state.ArgumentParser.TryGet("update", out var isUpdate);

        state.IsUpdating = !string.IsNullOrEmpty(isUpdate);

        state.ArgumentParser.TryGet("platform", out var argPlatform);

        string platform = String.IsNullOrEmpty(argPlatform) ? "Project" : argPlatform;

        state.ArgumentParser.TryGet("user", out var user);

        state.User = user;

        state.ArgumentParser.TryGet("token", out var token);

        state.Token = token;

        char[] invalidChars = Path.GetInvalidPathChars();

        state.ArgumentParser.TryGet("repolist", out var list);

        state.GitSource = !String.IsNullOrEmpty(list) 
            ? list 
            : "";

        state.ArgumentParser.TryGet("build", out var build);

        string rootFolder = (!String.IsNullOrEmpty(build) && build.Any(c => invalidChars.Contains(c)))
            ? Path.Combine(Environment.CurrentDirectory, platform) 
            : build;

        state.RootFolder = rootFolder;

        string buildPath = Path.Combine(rootFolder, $"Projects");

        state.BuildPath = buildPath;

        string changelogPath = Path.Combine(rootFolder, "Changelogs");

        state.ChangelogPath = changelogPath;

    }
}