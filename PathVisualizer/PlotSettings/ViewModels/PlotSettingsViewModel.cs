using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SettingsService;

namespace PlotSettings.ViewModels
{
    public class PlotSettingsViewModel : BindableBase
    {
        public string BackgroundImage
        {
            get => Settings.BackgroundImage;
            set
            {
                if(value == BackgroundImage) return;

                Settings.BackgroundImage = value;
                RaisePropertyChanged();
            }
        }
        public PlotSettingsEventModel Settings { get; set; }
        public ObservableCollection<string> Colors { get; set; }
        
        public DelegateCommand BrowseImageCommand { get; set; }
        public DelegateCommand<Window> OkCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }
        public DelegateCommand ApplyCommand { get; set; }
        
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlotSettingService _plotSettingService;

        public PlotSettingsViewModel(IEventAggregator eventAggregator, IPlotSettingService plotSettingService)
        {
            _eventAggregator = eventAggregator;
            _plotSettingService = plotSettingService;

            BrowseImageCommand = new DelegateCommand(BrowseImageAction);
            OkCommand = new DelegateCommand<Window>(OkAction);
            CancelCommand = new DelegateCommand<Window>(CancelAction);
            ApplyCommand = new DelegateCommand(ApplyAction);

            Colors = new ObservableCollection<string>();
            Settings = new PlotSettingsEventModel();

            var colorType = typeof(Color);
            var propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (var c in propInfoList)
            {
                Colors.Add(c.Name);
            }

            Settings = _plotSettingService.LoadPlotSettings();
        }


        private void BrowseImageAction()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false, Filter = "Image files|*.png;*.PNG;*.bmp;*.BMP"
            };
            if (openFileDialog.ShowDialog() != true) return;

            var fileNames = openFileDialog.FileNames;

            if (fileNames.Any(fileName => !File.Exists(fileName))) return;

            BackgroundImage = fileNames.FirstOrDefault();
        }

        private void OkAction(Window window)
        {
            _plotSettingService.SavePlotSettings(Settings);
            _eventAggregator.GetEvent<PlotSettingsEvent>().Publish(Settings);
            window.Close();
        }

        private void CancelAction(Window window)
        {
            window.Close();
        }

        private void ApplyAction()
        {
            _plotSettingService.SavePlotSettings(Settings);
            _eventAggregator.GetEvent<PlotSettingsEvent>().Publish(Settings);
        }

        
    }
}
