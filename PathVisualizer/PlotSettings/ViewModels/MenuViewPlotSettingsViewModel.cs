using System.Windows;
using PlotSettings.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace PlotSettings.ViewModels
{
    public class MenuViewPlotSettingsViewModel : BindableBase
    {
        public DelegateCommand OpenPlotSettingsCommand { get; set; }
        private readonly PlotSettingsViewModel _plotSettingsViewModel;
        private PlotSettingsWindow _plotSettingsWindow;


        public MenuViewPlotSettingsViewModel(IEventAggregator eventAggregator)
        {
            OpenPlotSettingsCommand = new DelegateCommand(OpenPlotSettingsAction);
            _plotSettingsViewModel = new PlotSettingsViewModel(eventAggregator);
        }

        private void OpenPlotSettingsAction()
        {
            _plotSettingsWindow = new PlotSettingsWindow
            {
                DataContext = _plotSettingsViewModel
            };
            _plotSettingsWindow.Show();
        }
    }
}
