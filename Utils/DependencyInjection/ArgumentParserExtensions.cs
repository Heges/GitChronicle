using Utils.Utils;

namespace Utils.DependencyInjection
{
    public static class ArgumentParserExtensions
    {
        public static IEnumerable<KeyValuePair<string, string?>> RegisterArgsToConfiguration(this ArgumentParser parser)
        {
            var dict = parser.Get();

            var pairs = new List<KeyValuePair<string, string?>>();
            //BuildOptions
            if (dict.TryGetValue("update", out var update))
            {
                pairs.Add(new("Build:IsUpdating", true.ToString()));
            }
            if (dict.TryGetValue("platform", out var platform)) pairs.AddRange(Emit("Build", "Platform", platform));
            if (dict.TryGetValue("build", out var build))
            {
                char[] invalidChars = Path.GetInvalidPathChars();
                string rootFolder = (!String.IsNullOrEmpty(build) && build.Any(c => invalidChars.Contains(c)))
                    ? Path.Combine(Environment.CurrentDirectory, !String.IsNullOrEmpty(platform) ? platform : "Project")
                    : build;
                string buildPath = Path.Combine(rootFolder, $"Projects");
                string changelogPath = Path.Combine(rootFolder, "Changelogs");
                pairs.AddRange(Emit("Build", "RootFolder", rootFolder));
                pairs.AddRange(Emit("Build", "BuildPath", buildPath));
                pairs.AddRange(Emit("Build", "ChangelogPath", changelogPath));

                if (!String.IsNullOrEmpty(update))
                {
                    var cleanPaths = new List<string>()
                    {
                        Path.GetFullPath(Path.Combine(rootFolder, "Changelogs"))
                    };
                    pairs.AddRange(Emit("Build", "CleanPaths", cleanPaths));
                }
                else
                {
                    var cleanPaths = new List<string>()
                    {
                        Path.GetFullPath(buildPath),
                        Path.GetFullPath(Path.Combine(rootFolder, "Changelogs"))
                    };
                    pairs.AddRange(Emit("Build", "CleanPaths", cleanPaths));
                }

            }
            if (dict.TryGetValue("repolist", out var source)) pairs.AddRange(Emit("Build", "GitSource", source));
            
            //GitOptions
            if (dict.TryGetValue("since", out var sinceStr)) pairs.AddRange(Emit("Git", "StartDate", sinceStr));
            if (dict.TryGetValue("user", out var user)) pairs.AddRange(Emit("Git", "User", user));
            if (dict.TryGetValue("token", out var token)) pairs.AddRange(Emit("Git", "Token", token));

            return pairs;
        }

        private static IEnumerable<KeyValuePair<string, string?>> Emit(string prefix, string key, object value)
        {
            if (value is IEnumerable<string> list)
            {
                int i = 0;
                foreach (var v in list)
                    yield return new KeyValuePair<string, string?>($"{prefix}:{key}:{i++}", v);
                yield break;
            }

            yield return new KeyValuePair<string, string?>($"{prefix}:{key}", Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
