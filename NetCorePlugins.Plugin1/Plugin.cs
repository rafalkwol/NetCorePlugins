using NetCorePlugins.Plugin1.Dependency;
using NetCorePlugins.Shared;

namespace NetCorePlugins.Plugin1
{
    public class Plugin : IPlugin
    {
        private readonly Repository repository = new();

        public void Dispose()
        {

        }

        public void Initialize()
        {
        }

        public void Execute(Command command)
        {
            var value = this.repository.GetValue(command.Body);
        }
    }
}