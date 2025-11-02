using Changeloger.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Utils.Core.Models;
using Microsoft.Extensions.Logging;

namespace Changeloger.Services
{
    public class LoadChangelog
    {
        private readonly ILogger<LoadChangelog> _logger;

        public LoadChangelog(ILogger<LoadChangelog> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, Changelog> Load(string rootPath, string platformName)
        {
            Console.WriteLine("Загружаем чейнджлоги...");
            string directoryPath = Path.Combine(rootPath, "Changelogs");
            if (!Directory.Exists(directoryPath))
                throw new Exception($"LoadChangelogs:Load Directory {directoryPath} doesnt exist;");
            //string changelogRoot = Path.Combine(rootPath, Path.Combine(platformName, "Changelogs"));
            List<Commit> commits = new List<Commit>();
            Dictionary<string, Changelog> changelogs = new Dictionary<string, Changelog>();
            try
            {
                var settings = new JsonSerializerSettings();
                // Указываем формат даты. Формат "yyyy-MM-ddTHH:mm:ssK" подходит для строки вида "2025-02-25T11:55:13+03:00"
                settings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddTHH:mm:ssK" });
                var txtFilesAll = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);

                foreach (var file in txtFilesAll)
                {
                    try
                    {
                        var line = File.ReadAllText(file);
                        var listCommits = JsonConvert.DeserializeObject<List<Commit>>(line, settings);
                        if (file != null && file.Length > 3)
                        {
                            string[] parts = file.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            string projectName = parts[parts.Length - 1];

                            Changelog changelog = null;

                            if (listCommits != null && listCommits.Count > 0)
                            {
                                changelog = new Changelog(platformName, projectName, listCommits.Select(x =>
                                {
                                    var changeLogItem = new ChangelogItem();

                                    changeLogItem.ChangelogItemHashCommit = x.Hash;
                                    changeLogItem.ChangelogItemDate = x.Date;
                                    changeLogItem.ChangelogItemDescription = x.Body;
                                    var splitedArray = x.Subject.Split(':');
                                    var splitedSubj = splitedArray.Skip(1).FirstOrDefault();
                                    var splitedType = splitedArray.Skip(0).FirstOrDefault();
                                    changeLogItem.ChangelogItemTitle = String.IsNullOrEmpty(splitedSubj) ? x.Subject : splitedSubj;
                                    changeLogItem.ChangelogItemTypeCommit = String.IsNullOrEmpty(splitedType) ? "" : splitedType;
                                    changeLogItem.ChangelogItemAuthor = x.Author;

                                    return changeLogItem;
                                }).ToList());
                            }
                            changelogs.Add(projectName, changelog ?? new Changelog("", "", new List<ChangelogItem>()));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erorr: LoadConfigurationService, LoadFiles, json seriliazation: {ex.StackTrace}.");
                    }
                }

                //string outputFile = Path.Combine(changelogRoot, $"changelog-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt");

                return changelogs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erorr: LoadConfigurationService, LoadFiles: {ex.StackTrace}.");
            }
            return new Dictionary<string, Changelog>();
        }
    }
}
