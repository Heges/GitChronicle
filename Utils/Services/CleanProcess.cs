using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;

namespace Utils.Services
{
    public class CleanProcess : IProcess
    {
        private readonly IState _state;
        private readonly IShell _shell;
        private readonly ILogger _logger;

        public CleanProcess(IState state, IShell shell, ILogger logger)
        {
            _state = state;
            _shell = shell;
            _logger = logger;
        }

        public Task<bool> Process()
        {
            if (String.IsNullOrEmpty(_state.BuildPath))
            {
                _logger.LogError("Параметр --build обязателен.");
                return Task.FromResult(false);
            }

            var buildPath = Path.GetFullPath(_state.BuildPath);
            if (!Directory.Exists(buildPath))
            {
                _logger.LogError($"Каталог не найден: {buildPath}");
                return Task.FromResult(false);
            }

            var entries = Directory.EnumerateFileSystemEntries(buildPath, "*", 
                new EnumerationOptions 
                { 
                    RecurseSubdirectories = false, 
                    AttributesToSkip = 0, 
                    IgnoreInaccessible = true 
            });

            foreach (var entry in entries)
            {
                try 
                { 
                    ForceDelete(entry); 
                }
                catch (Exception ex) 
                {
                    _logger.LogError($"Не удалось удалить: {entry} — {ex.Message}"); 
                }
            }

            Console.WriteLine("Очистка завершена");
            return Task.FromResult(true);
        }

        private void ForceDelete(string path)
        {
            if (Directory.Exists(path)) 
            { 
                ForceDeleteDirectory(path);
                return; 
            }
            if (File.Exists(path))
            { 
                ForceDeleteFile(path); 
                return;
            }
        }

        private  void ForceDeleteDirectory(string dir, int retries = 6, int delayMs = 250)
        {
            TryNormalizeAttributesRecursive(dir);

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    Directory.Delete(dir, recursive: true);
                    if (!Directory.Exists(dir)) 
                        return;
                }
                catch (Exception ex) {
                    _logger.LogError($"При удалении произошла ошибка. {ex.Message}");
                }
                Thread.Sleep(delayMs);
            }
        }

        private void ForceDeleteFile(string file, int retries = 6, int delayMs = 250)
        {
            TrySetFileAttributes(file, FileAttributes.Normal);

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    File.Delete(file);
                    if (!File.Exists(file)) 
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"При удалении произошла ошибка. {ex.Message}");
                }

                Thread.Sleep(delayMs);
            }
        }

        private void TryNormalizeAttributesRecursive(string root)
        {
            foreach (var f in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                TrySetFileAttributes(f, FileAttributes.Normal);

            foreach (var d in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories))
                TrySetDirAttributes(d, FileAttributes.Normal);

            TrySetDirAttributes(root, FileAttributes.Normal);
        }

        private void TrySetFileAttributes(string path, FileAttributes attrs)
        {
            try 
            { 
                File.SetAttributes(path, attrs);
            } 
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при выставлении аттрибутов {ex.Message}.");
            }
        }

        private void TrySetDirAttributes(string path, FileAttributes attrs)
        {
            try
            {
                new DirectoryInfo(path).Attributes = attrs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при выставлении аттрибутов {ex.Message}.");
            }
        }
    }
}
