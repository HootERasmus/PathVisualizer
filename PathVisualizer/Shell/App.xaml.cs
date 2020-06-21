using Prism.Ioc;
using Shell.Views;
using System.Windows;
using PipelineService;
using Prism.Modularity;

namespace Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IPipeline, Pipeline>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            var catalog = new DirectoryModuleCatalog {ModulePath = @".\ExtensionModules"};
            catalog.Load();
            
            foreach (var module in catalog.Modules)
            {
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = module.ModuleName,
                    ModuleType = module.ModuleType,
                    InitializationMode = InitializationMode.WhenAvailable
                });
            }
        }
    }
}
