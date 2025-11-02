using Microsoft.Extensions.DependencyInjection;
using Utils.Core;
using Utils.Core.Enum;
using Utils.Core.Interfaces;
using Utils.Core.Models;
using Utils.Services;

namespace Changloger
{
    public class Pipeline
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IContext _state;

        private Dictionary<string, IProcess> _map;

        private List<ESteps> _pipeline;

        public Pipeline(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            _serviceProvider = scope.ServiceProvider;

            _state = _serviceProvider.GetRequiredService<IContext>();

            _map = new Dictionary<string, IProcess>()
            {
                { "Clean", _serviceProvider.GetRequiredService<Clean>() },
                { "Update", _serviceProvider.GetRequiredService<GitUpdate>() },
                { "Init", _serviceProvider.GetRequiredService<GitInit>() },
                { "Log", _serviceProvider.GetRequiredService<GitLog>() },
            };

            _pipeline = new List<ESteps>() { ESteps.Clean, ESteps.Update, ESteps.Init, ESteps.Log };
        }

        public async Task<bool> Run(CancellationToken token = default)
        {
            foreach (var step in _pipeline)
            {
                if (_map.TryGetValue(Enum.GetName(step)!, out var process) && process is IProcess)
                {
                    var result = await process.Process(token);
                    Info(result, process);
                }
            }
            return true;
        }

        public void Info(Result result, IProcess process)
        {
            Console.WriteLine(result.IsSuccess ? $"Этап {process.Name}: {string.Join(", ", result.Messages)}" : $"Этап завершился с ошибкой: {string.Join(", ", result.Errors)}");
        }
    }
}
