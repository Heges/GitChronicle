using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Core.Models;

namespace Utils.Services
{
    public class GitUpdate : IProcess
    {
        private readonly IContext _state;
        private readonly IShell _shell;
        private readonly ILogger<GitUpdate> _logger;

        public string Name { get; }

        public GitUpdate(IContext state, IShell shell, ILogger<GitUpdate> logger)
        {
            _state = state;
            _shell = shell;
            _logger = logger;
            Name = "Uptdate";
        }

        public async Task<Result> Process(CancellationToken ct = default)
        {
            var result = new Result();

            if (_state.IsUpdating)
            {
                if (String.IsNullOrEmpty(_state.BuildOptions.BuildPath))
                {
                    return result.Fail("Параметр --build обязателен.");
                }

                var buildPath = Path.GetFullPath(_state.BuildOptions.BuildPath);
                if (!Directory.Exists(buildPath))
                {
                    return result.Fail($"Каталог не найден: {buildPath}");
                }

                foreach (var dir in Directory.EnumerateDirectories(buildPath))
                {
                    if (Directory.Exists(Path.Combine(dir, ".git")))
                    {
                        Console.WriteLine($"Pull в {dir}");
                        var rc = await _shell.Run(dir, ["pull", "--ff-only"]);
                        if (rc != 0)
                            _logger.LogError($"Не удалось обновить {dir} (код {rc})");
                    }
                }
                return result.Ok(new Dictionary<string, object> { { Name, this } }, $"Обновление проектов прошло успешно: {buildPath}");
            }
            return result.Ok(new Dictionary<string, object> { { Name, this } }, $"Обновление проектов пропущена.");
        }
    }
}
