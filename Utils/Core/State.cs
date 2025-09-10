using Utils.Core.Interfaces;
using Utils.Utils;

namespace Utils.Core
{
    public class State : IState
    {
        public bool IsRunning { get; set; }
        public bool IsUpdating { get; set; }

        public string RootFolder { get; set; } = String.Empty;
        public string BuildPath { get; set; } = String.Empty;
        public string ChangelogPath { get; set; } = String.Empty;
        public string GitSource { get; set; } = String.Empty;  
        public string Platform { get; set; } = String.Empty;
        public string User { get; set; } = String.Empty;
        public string Token { get; set; } = String.Empty;

        public ArgumentParser ArgumentParser { get; set; }
    }
}
