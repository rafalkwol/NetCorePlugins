using System;

namespace NetCorePlugins.Shared
{
    public interface IPlugin : IDisposable
    {
        void Initialize();

        void Execute(Command command);
    }
}