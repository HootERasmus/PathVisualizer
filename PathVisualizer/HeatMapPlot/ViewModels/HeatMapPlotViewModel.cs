using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;
using OxyPlot;
using PipelineService;
using PlotModelService;
using Prism.Events;
using SettingsService;

namespace HeatMapPlot.ViewModels
{
    public class HeatMapPlotViewModel : BindableBase
    {
        public PlotModel MyPlotModel { get; set; }
        public PlotSettingsEventModel Settings { get; set; }
        public Tag SelectedTag { get; set; }

        public ObservableCollection<string> PipelineHistory { get; set; }

        private string _backgroundImage;
        public string BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                if (value == BackgroundImage) return;

                _backgroundImage = value;
                RaisePropertyChanged();
            }
        }

        private readonly IPlotModelHelper _plotModelHelper;

        public HeatMapPlotViewModel(IEventAggregator eventAggregator, IPlotModelHelper plotModelHelper, IPlotSettingService plotSettingService)
        {
            PipelineHistory = new ObservableCollection<string>();

            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);

            _plotModelHelper = plotModelHelper;
            Settings = plotSettingService.LoadPlotSettings();
            ApplyPlotSettings(Settings);
        }

        private async void OnPipelineCompletedEvent(IList<PipelineCompletedEventModel> history)
        {
            PipelineHistory.Clear();

            foreach (var tag in history)
            {
                PipelineHistory.Add($"{tag.Name} - {tag.Tag.TimeCoordinates.Count}");
            }

            if (history.Any())
                await PlotHeatMap(history.Last().Tag);
        }

        private async Task PlotHeatMap(Tag tag)
        {
            SelectedTag = tag;
            MyPlotModel = await _plotModelHelper.PlotTagOnHeatMapPlotModel(MyPlotModel, SelectedTag, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }

        private async void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            BackgroundImage = Settings.BackgroundImage;
            
            MyPlotModel = new PlotModel();

            MyPlotModel = _plotModelHelper.ApplyHeatMapPlotSettings(MyPlotModel, Settings);
            
            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                await PlotHeatMap(SelectedTag);
            }
        }
    }
}
