using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Lib.Events;
using Lib.SharedModels;
using MetadataExtractor;
using Microsoft.Win32;
using OxyPlot;
using PipelineService;
using Prism.Commands;
using Prism.Events;
using Tag = Lib.SharedModels.Tag;

namespace LinePlot.ViewModels
{
    public class MenuFileExportViewModel
    {
        public DelegateCommand ExportCommand { get; set; }
        private Tag _tag;
        private PlotSettingsEventModel _settings;
        private int _backgroundHeight;
        private int _backgroundWidth;

        public MenuFileExportViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(history => _tag = history.Values.Last());
            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(SaveSettings);
            _backgroundHeight = 800;
            _backgroundWidth = 600;

            ExportCommand = new DelegateCommand(ExportAction);
        }

        private void SaveSettings(PlotSettingsEventModel model)
        {
            _settings = model;

            try
            {
                var directories = ImageMetadataReader.ReadMetadata(_settings.BackgroundImage);

                _backgroundWidth = int.Parse(directories[0].Tags[0].Description);
                _backgroundHeight = int.Parse(directories[0].Tags[1].Description);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        
        private async void  ExportAction()
        {
            if (_tag == null)
            {
                MessageBox.Show("No tag was selected");
                return;
            }

            var dialog = new SaveFileDialog
            {
                DefaultExt = ".png",
                Filter = "*.png|*.PNG",
                AddExtension = true,
                FileName = $"{_tag.Id}"
            };
            dialog.ShowDialog(Application.Current.MainWindow);

            if (string.IsNullOrEmpty(dialog.FileName)) return;

            var plotModel = new PlotModel();
            plotModel = await PlotModelHelper.ApplySettings(plotModel, _settings);
            plotModel = await PlotModelHelper.PlotTagOnPlotModel(plotModel, _tag, _settings);
            PlotModelHelper.ExportImage(plotModel, dialog.FileName, _backgroundHeight, _backgroundWidth);
        }
    }
}
