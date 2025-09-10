using Utils.Utils;

namespace Utils.Core.Interfaces
{
    public interface IState
    {
        public bool IsRunning { get; set; }
        public bool IsUpdating { get; set; }

        public string RootFolder { get; set; }
        public string BuildPath { get; set; }
        public string ChangelogPath { get; set; }
        public string GitSource { get; set; }
        public string Platform { get; set; }
        public string User { get; set; }
        public string Token { get; set; }

        public ArgumentParser ArgumentParser { get; set; }
    }
}
