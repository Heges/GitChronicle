using System.Text;
using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Core.Models;
using Utils.Utils;

namespace Utils.Services
{
    public class GitInit : IProcess
    {
        private readonly IContext _state;
        private readonly IShell _shell;
        private readonly ILogger<GitInit> _logger;

        public string Name { get; }

        public GitInit(IContext state, IShell shell, ILogger<GitInit> logger)
        {
            _state = state;
            _shell = shell;
            _logger = logger;
            Name = "Init";
        }

        public async Task<Result> Process(CancellationToken ct = default)
        {
            var result = new Result();
            if (!_state.IsUpdating)
            {
                if (String.IsNullOrEmpty(_state.GitOptions.User))
                {
                    return result.Fail("Параметр --username обязателен.");
                }

                if (String.IsNullOrEmpty(_state.GitOptions.Token))
                {
                    return result.Fail("Параметр --token обязателен.");
                }

                if (String.IsNullOrEmpty(_state.BuildOptions.BuildPath))
                {
                    return result.Fail("Параметр --build обязателен.");
                }

                var gitGroupPath = false;

                if (String.IsNullOrEmpty(_state.BuildOptions.GitSource))
                {
                    Console.WriteLine($"Параметр --repolist не задан, будем использоваться группа {_state.BuildOptions.Platform}.");

                    gitGroupPath = true;
                }

                var buildPath = Path.GetFullPath(_state.BuildOptions.BuildPath);

                string repoListPath = "Git";

                if (!gitGroupPath)
                    repoListPath = Path.GetFullPath(_state.BuildOptions.GitSource);

                if (!Directory.Exists(buildPath))
                    Directory.CreateDirectory(buildPath);
                if (!gitGroupPath)
                {
                    if (!File.Exists(repoListPath))
                    {
                        result.Fail($"Файл списка репозиториев не найден: {repoListPath}");
                    }
                }

                Console.WriteLine($"Каталог сборки: {buildPath}");
                Console.WriteLine($"Список репозиториев: {repoListPath}");

                if (!gitGroupPath)
                {
                    var lines = await File.ReadAllLinesAsync(repoListPath, Encoding.UTF8);
                    foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        var repo = line.TrimEnd(';');
                        if (!repo.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Пропуск не-HTTPS URL: {repo}");
                            continue;
                        }

                        var urlWithToken = repo.Replace("https://", $"https://{Uri.EscapeDataString(_state.GitOptions.User)}:{Uri.EscapeDataString(_state.GitOptions.Token)}@");
                        Console.WriteLine($"Клонирование {urlWithToken.HideSensivityText(new string[] { _state.GitOptions.User, _state.GitOptions.Token })}");

                        var rc = await _shell.Run(buildPath, ["clone", urlWithToken]);
                        if (rc != 0)
                        {
                            _logger.LogError($"Ошибка клонирования: {repo} (код {rc})");
                        }
                    }
                }
                else
                {
                    ///TO DO Получить здесь список проектов по группе
                }

                return result.Ok(new Dictionary<string, object> { { Name, this } }, $"Инициализация репозитория прошла успешна по пути: {buildPath}");
            }
            return result.Ok(new Dictionary<string, object> { { Name, this } }, $"Инициализация репозитория пропущена.");
        }
    }
}
