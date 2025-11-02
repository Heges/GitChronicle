using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Utils.Core.Interfaces;
using Utils.Core.Options;

namespace Utils.Core
{
    public class Context : IContext
    {
        public bool IsUpdating { get; private set; }
        public BuildOptions BuildOptions { get; private set; }
        public GitOptions GitOptions { get; private set; }

        public Context(IConfiguration configuration, IOptions<BuildOptions> buildOptions, IOptions<GitOptions> gitOptions)
        {
            IsUpdating = bool.TryParse(configuration["Build:IsUpdating"], out var result);
            BuildOptions = buildOptions.Value;
            GitOptions = gitOptions.Value;
        }
    }
}
