using Microsoft.Extensions.DependencyInjection;
using Utils.Core;
using Utils.Core.Interfaces;

namespace Changloger
{
    public class UtilsEntryPoint
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IState _state;

        public UtilsEntryPoint(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _state = _serviceProvider.GetRequiredService<IState>();
        }

        public async Task<bool> Run()
        {
            using var scope = _serviceProvider.CreateScope();

            var factory = scope.ServiceProvider.GetRequiredService<Func<string, IProcess>>();

            if (_state.IsUpdating)
            {
                var updateService = factory("update");
                var ur = await updateService.Process();
                var changelogService = factory("changelog");
                var ch = await changelogService.Process();
            }
            else
            {
                var cleanService = factory("clean");
                var cr = await cleanService.Process();
                Console.WriteLine(cr ? "Очистка завершена успешно." : "Очистка завершилась с ошибкой.");
                var initService = factory("init");
                var ir = await initService.Process();
                
                var changelogService = factory("changelog");
                var ch = await changelogService.Process();

                return false;
            }

            return true;
        }
    }
}
