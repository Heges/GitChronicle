using Utils.Core.Options;

namespace Utils.Core.Interfaces
{
    public interface IContext
    {
        public bool IsUpdating { get; }
        public BuildOptions BuildOptions { get;  }
        public GitOptions GitOptions { get;  }

    }
}
