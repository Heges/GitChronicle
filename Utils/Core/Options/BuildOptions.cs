namespace Utils.Core.Options
{
    public class BuildOptions
    {
        public string RootFolder { get; set; } = String.Empty;
        public string BuildPath { get; set; } = String.Empty;
        public string ChangelogPath { get; set; } = String.Empty;
        public string GitSource { get; set; } = String.Empty;
        public string Platform { get; set; } = String.Empty;
        public IReadOnlyList<string> CleanPaths { get;  set; } = new List<string>();
    }
}
