using TimePicker.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TimePicker
{
    public class TimePickerModule : IModule
    {
        public TimePickerModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.TimePickingRegion, typeof(TimePickerView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<TimePickerView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<TimePickerView>();
        }
    }
}