using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using McMaster.NETCore.Plugins;
using NetCorePlugins.Shared;

namespace NetCorePlugins.Host
{
    internal class Program
    {
        private static async Task Main()
        {
            var applicationDirectory = Directory.GetCurrentDirectory();

            var pluginFilePaths = new[]
                                      {
                                          $@"{applicationDirectory}\Plugins\Plugin1\NetCorePlugins.Plugin1.dll",
                                          $@"{applicationDirectory}\Plugins\Plugin2\NetCorePlugins.Plugin2.dll"
                                      };

            foreach (var pluginFilePath in pluginFilePaths)
            {
                ExecuteAndUnload(pluginFilePath, out var pluginLoaderReference);

                var counter = 0;

                while (pluginLoaderReference.IsAlive || PluginStillLoaded())
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    await Task.Delay(1000);

                    counter++;

                    Console.WriteLine(counter);
                }
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ExecuteAndUnload(string pluginFilePath, out WeakReference pluginLoaderReference)
        {
            using var pluginLoader = PluginLoader.CreateFromAssemblyFile(pluginFilePath, new[] { typeof(IPlugin) }, x => { x.IsUnloadable = true; });

            pluginLoaderReference = new WeakReference(pluginLoader);

            foreach (var type in pluginLoader.LoadDefaultAssembly().GetTypes().Where(x => typeof(IPlugin).IsAssignableFrom(x) && !x.IsAbstract))
            {
                using var plugin = (IPlugin)Activator.CreateInstance(type);

                plugin!.Execute(new Command { Body = "Server=localhost;Database=master;User Id=sa;Password=sa;TrustServerCertificate=True;" });
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool PluginStillLoaded() =>
            AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName?.Contains("NetCorePlugins.Plugin") ?? false) != null;
    }
}