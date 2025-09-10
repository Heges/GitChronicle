using System.Text;
using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Utils;

namespace Utils.Services
{
    public class InitProcess : IProcess
    {
        private readonly IState _state;
        private readonly IShell _shell;
        private readonly ILogger _logger;

        public InitProcess(IState state, IShell shell, ILogger logger)
        {
            _state = state;
            _shell = shell;
            _logger = logger;
        }

        public async Task<bool> Process()
        {
            if (String.IsNullOrEmpty(_state.User))
            {
                _logger.LogError("Параметр --username обязателен.");
                return false;
            }

            if (String.IsNullOrEmpty(_state.Token))
            {
                _logger.LogError("Параметр --token обязателен.");
                return false;
            }

            if (String.IsNullOrEmpty(_state.BuildPath))
            {
                _logger.LogError("Параметр --build обязателен.");
                return false;
            }

            var gitGroupPath = false;

            if (String.IsNullOrEmpty(_state.GitSource))
            {
                Console.WriteLine($"Параметр --repolist не задан, будем использоваться группа {_state.Platform}.");

                gitGroupPath = true;
            }

            var buildPath = Path.GetFullPath(_state.BuildPath);

            string repoListPath = "Git";

            if (!gitGroupPath)
                repoListPath = Path.GetFullPath(_state.GitSource);

            if (!Directory.Exists(buildPath))
                Directory.CreateDirectory(buildPath);
            if (!gitGroupPath)
            {
                if (!File.Exists(repoListPath))
                {
                    _logger.LogError($"Файл списка репозиториев не найден: {repoListPath}");
                    return false;
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

                    var urlWithToken = repo.Replace("https://", $"https://{Uri.EscapeDataString(_state.User)}:{Uri.EscapeDataString(_state.Token)}@");
                    Console.WriteLine($"Клонирование {urlWithToken.HideSensivityText(new string[] { _state.User, _state.Token })}");

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

            return true;
        }
    }
}
