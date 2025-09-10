using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;

namespace Utils.Services
{
    public class UpdateProcess : IProcess
    {
        private readonly IState _state;
        private readonly IShell _shell;
        private readonly ILogger _logger;

        public UpdateProcess(IState state, IShell shell, ILogger logger)
        {
            _state = state;
            _shell = shell;
            _logger = logger;
        }

        public async Task<bool> Process()
        {
            if (String.IsNullOrEmpty(_state.BuildPath))
            {
                _logger.LogError("Параметр --build обязателен.");
                return false;
            }

            var buildPath = Path.GetFullPath(_state.BuildPath);
            if (!Directory.Exists(buildPath))
            {
                _logger.LogError($"Каталог не найден: {buildPath}");
                return false;
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

            return true;
        }
    }
}
