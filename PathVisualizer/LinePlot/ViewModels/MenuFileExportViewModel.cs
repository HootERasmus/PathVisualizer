using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Lib.Events;
using Lib.SharedModels;
using MetadataExtractor;
using Microsoft.Win32;
using OxyPlot;
using PipelineService;
using PlotModelService;
using Prism.Commands;
using Prism.Events;
using SettingsService;
using Tag = Lib.SharedModels.Tag;

namespace LinePlot.ViewModels
{
    public class MenuFileExportViewModel
    {
        public DelegateCommand ExportCommand { get; set; }

        private readonly IPlotModelHelper _plotModelHelper;

        private Tag _tag;
        private PlotSettingsEventModel _settings;
        private int _backgroundHeight;
        private int _backgroundWidth;

        public MenuFileExportViewModel(IEventAggregator eventAggregator, IPlotModelHelper plotModelHelper)
        {
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(SaveSettings);
            _backgroundHeight = 800;
            _backgroundWidth = 600;
            _plotModelHelper = plotModelHelper;

            ExportCommand = new DelegateCommand(ExportAction);
        }

        private void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            if (history.Values.Any())
                _tag = history.Values.Last();
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
            plotModel = _plotModelHelper.ApplyLinePlotSettings(plotModel, _settings);
            plotModel = await _plotModelHelper.PlotTagOnLinePlotModel(plotModel, _tag, _settings);
            _plotModelHelper.ExportImage(plotModel, dialog.FileName, _backgroundHeight, _backgroundWidth);
        }
    }
}
