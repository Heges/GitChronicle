using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Utils.Core;
using Utils.Core.Interfaces;
using Utils.Core.Models;

namespace Utils.Services
{
    public class ChangelogProcess : IProcess
    {
        private readonly IState _state;
        private readonly IShell _shell;
        private readonly ILogger _logger;

        public ChangelogProcess(IState state, IShell shell, ILogger logger)
        {
            _state = state;
            _shell = shell;
            _logger = logger;
        }

        public async Task<bool> Process()
        {
            if (String.IsNullOrEmpty(_state.ChangelogPath))
            {
                _logger.LogError("Параметр --changelog обязателен.");
                return false;
            }
            if (String.IsNullOrEmpty(_state.BuildPath))
            {
                _logger.LogError("Параметр --build обязателен.");
                return false;
            }
            if (!_state.ArgumentParser.TryGet("since", out var sinceStr) || string.IsNullOrWhiteSpace(sinceStr))
            {
                _logger.LogError("Параметр --since обязателен (напр. 2025-08-01).");
                return false;
            }
            

            var buildPath = Path.GetFullPath(_state.BuildPath);
            var changelogPath = Path.GetFullPath(_state.ChangelogPath);
            var sinceDate = sinceStr;

            if (!Directory.Exists(changelogPath))
                Directory.CreateDirectory(changelogPath);

            Console.WriteLine($"Сбор changelog'ов с {sinceDate}");
            //раздитель по полям
            char fs = '\x1f';
            //разделитель по записи
            char rs = '\x1e';

            foreach (var dir in Directory.EnumerateDirectories(buildPath))
            {
                var name = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                
                if (!Directory.Exists(Path.Combine(dir, ".git")))
                    continue;

                Console.WriteLine($"Обработка: {name}");
                // Форматированный лог для парсинга c разделителями
                var format = $"%H%x1f%an%x1f%ad%x1f%B%x1e";
                var (rc, stdout, stderr) = await _shell.RunInfo(dir, ["log", $"--since={sinceDate}", $"--pretty=format:{format}", "--date=iso"]);

                if (rc != 0)
                {
                    _logger.LogError($"git log ошибка в {name}: {stderr}");
                    continue;
                }

                var commits = new List<Commit>();

                var records = stdout.Split(rs, StringSplitOptions.RemoveEmptyEntries);

                foreach (var record in records)
                {
                    var fields = record.Split(fs);

                    if (fields.Length < 4)
                        continue;

                    var hash = fields[0];
                    var author = fields[1];
                    var date = fields[2];
                    var message = fields[3].Replace("\r\n", "\n");

                    string subject, body;
                    var nl = message.IndexOf('\n');
                    if (nl >= 0)
                    {
                        subject = message[..nl];
                        body = message[(nl + 1)..].Trim('\n', '\r');
                    }
                    else
                    {
                        subject = message;
                        body = string.Empty;
                    }

                    if (body.StartsWith("\n"))
                        body = body.TrimStart('\n');

                    commits.Add(new Commit
                    {
                        Hash = hash,
                        Author = author,
                        Date = date,
                        Subject = subject,
                        Body = body
                    });
                }

                var json = JsonSerializer.Serialize(commits, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var outFile = Path.Combine(changelogPath, $"{SanitizeFileName(name)}.json");
                await File.WriteAllTextAsync(outFile, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

                Console.WriteLine($"Changelog сохранён: {outFile}");
            }

            return true;
        }

        private string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
