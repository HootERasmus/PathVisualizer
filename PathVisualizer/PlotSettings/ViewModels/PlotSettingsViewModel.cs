using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Lib.Events;
using Lib.SharedModels;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

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

        public PlotSettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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

            Settings = LoadSettings();

            ApplyCommand.Execute();
        }


        private void BrowseImageAction()
        {
            var openFileDialog = new OpenFileDialog { Multiselect = false };
            openFileDialog.Filter = "Image files|*.png;*.PNG;*.bmp;*.BMP";
            if (openFileDialog.ShowDialog() != true) return;

            var fileNames = openFileDialog.FileNames;

            if (fileNames.Any(fileName => !File.Exists(fileName))) return;

            BackgroundImage = fileNames.FirstOrDefault();
        }

        private void OkAction(Window window)
        {
            SaveSettings(Settings);
            _eventAggregator.GetEvent<PlotSettingsEvent>().Publish(Settings);
            window.Close();
        }

        private void CancelAction(Window window)
        {
            window.Close();
        }

        private void ApplyAction()
        {
            SaveSettings(Settings);
            _eventAggregator.GetEvent<PlotSettingsEvent>().Publish(Settings);
        }

        private void SaveSettings(PlotSettingsEventModel model)
        {
            UserSettings.Default.XAxisTitle = model.XAxisTitle;
            UserSettings.Default.YAxisTitle = model.YAxisTitle;

            UserSettings.Default.XAxisMinimum = model.XAxisMinimum;
            UserSettings.Default.XAxisMaximum = model.XAxisMaximum;

            UserSettings.Default.YAxisMinimum = model.YAxisMinimum;
            UserSettings.Default.YAxisMaximum = model.YAxisMaximum;

            UserSettings.Default.BackgroundImagePath = model.BackgroundImage;

            UserSettings.Default.LineColor = model.LineColor;
            UserSettings.Default.DotColor = model.DotColor;

            UserSettings.Default.Save();

        }

        private PlotSettingsEventModel LoadSettings()
        {
            var model = new PlotSettingsEventModel
            {
                XAxisTitle = UserSettings.Default.XAxisTitle,
                YAxisTitle = UserSettings.Default.YAxisTitle,
                XAxisMinimum = UserSettings.Default.XAxisMinimum,
                XAxisMaximum = UserSettings.Default.XAxisMaximum,
                YAxisMinimum = UserSettings.Default.YAxisMinimum,
                YAxisMaximum = UserSettings.Default.YAxisMaximum,
                BackgroundImage = UserSettings.Default.BackgroundImagePath,
                LineColor = UserSettings.Default.LineColor,
                DotColor = UserSettings.Default.DotColor
            };
            
            return model;
        }
    }
}
