namespace Utils.Core.Interfaces
{
    public interface IShell
    {
        public Task<int> Run(string workingDir, IReadOnlyList<string> args);
        public Task<(int rc, string stdout, string stderr)> RunInfo(string workingDir, IReadOnlyList<string> args);
    }
}
