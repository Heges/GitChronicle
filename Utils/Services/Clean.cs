using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Core.Models;

namespace Utils.Services
{
    public class Clean : IProcess
    {
        private readonly IContext _state;
        private readonly ILogger<Clean> _logger;

        public string Name { get; }

        public Clean(IContext state, ILogger<Clean> logger)
        {
            _state = state;
            _logger = logger;
            Name = "Clean";
        }

        public Task<Result> Process(CancellationToken ct = default)
        {
            var result = new Result();

            if (String.IsNullOrEmpty(_state.BuildOptions.BuildPath))
            {
                return Task.FromResult(result.Fail("Параметр --build обязателен."));
            }
            
            foreach (var clean in _state.BuildOptions.CleanPaths)
            {
                Console.WriteLine($"Очищаем директорию: {clean}");
                if (!Directory.Exists(clean))
                {
                    return Task.FromResult(result.Fail($"Каталог не найден: {clean}"));
                }

                var entries = Directory.EnumerateFileSystemEntries(clean, "*",
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
            }
            return Task.FromResult(result.Ok(new Dictionary<string, object> { { Name, this } }, $"Очистка завершена"));
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

        private void ForceDeleteDirectory(string dir, int retries = 6, int delayMs = 250)
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
