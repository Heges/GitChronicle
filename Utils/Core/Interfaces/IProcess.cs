using Utils.Core.Models;

namespace Utils.Core
{
    public interface IProcess
    {
        public string Name { get; }
        public Task<Result> Process(CancellationToken ct = default);
    }
}
