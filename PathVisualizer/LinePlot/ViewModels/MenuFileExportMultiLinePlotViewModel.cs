using System;
using System.Diagnostics;
using System.Linq;
using PipelineService;
using Prism.Commands;
using System.Windows.Forms;
using DataLoaderService;
using Lib.Events;
using Lib.SharedModels;
using LinePlot.Events;
using MetadataExtractor;
using OxyPlot;
using Prism.Events;

namespace LinePlot.ViewModels
{
    public class MenuFileExportMultiLinePlotViewModel
    {
        public DelegateCommand ExportCommand { get; set; }
        private readonly IPipeline _pipeline;
        private readonly IDataLoader _dataLoader;
        private readonly IEventAggregator _eventAggregator;
        private PlotSettingsEventModel _settings;
        private int _backgroundHeight;
        private int _backgroundWidth;

        public MenuFileExportMultiLinePlotViewModel(IPipeline pipeline, IDataLoader dataLoader, IEventAggregator eventAggregator)
        {
            _pipeline = pipeline;
            _dataLoader = dataLoader;
            _eventAggregator = eventAggregator;
            _backgroundHeight = 800;
            _backgroundWidth = 600;
            
            _eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(SaveSettings);

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

        private async void ExportAction()
        {
            using var fdb = new FolderBrowserDialog();

            var result = fdb.ShowDialog();

            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fdb.SelectedPath)) return;
            
            _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, _dataLoader.Tags.Count, 0));
            var count = 0;

            foreach (var tag in _dataLoader.Tags)
            {
                var history = await _pipeline.StartSilentPipeline(new PipelineStartEventModel(this, tag));
                var filteredTag = history.Values.Last();

                var plotModel = new PlotModel();
                plotModel = await PlotModelHelper.ApplySettings(plotModel, _settings);
                plotModel = await PlotModelHelper.PlotTagOnPlotModel(plotModel, filteredTag, _settings);
                PlotModelHelper.ExportImage(plotModel, $"{fdb.SelectedPath}/{tag.Id}.png", _backgroundHeight, _backgroundWidth);
                _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, _dataLoader.Tags.Count, ++count));
            }
        }
    }
}
