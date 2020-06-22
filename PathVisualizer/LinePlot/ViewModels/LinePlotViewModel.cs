using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OxyPlot;
using PipelineService;
using PlotModelService;
using Prism.Events;
using SettingsService;
using Tag = Lib.SharedModels.Tag;

namespace LinePlot.ViewModels
{
    public class LinePlotViewModel : BindableBase
    {
        public PlotModel MyPlotModel { get; set; }
        public PlotSettingsEventModel Settings { get; set; }
        public Tag SelectedTag { get; set; }
        public ObservableCollection<string> PipelineHistory { get; set; }

        private readonly IPlotModelHelper _plotModelHelper;
        
        public LinePlotViewModel(IEventAggregator eventAggregator, IPlotModelHelper plotModelHelper, IPlotSettingService plotSettingService)
        {
            _plotModelHelper = plotModelHelper;
            PipelineHistory = new ObservableCollection<string>();
            
            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);

            Settings = plotSettingService.LoadPlotSettings();
            ApplyPlotSettings(Settings);
        }

        private async void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            PipelineHistory.Clear();

            foreach (var tag in history)
            {
                PipelineHistory.Add($"{tag.Key} - {tag.Value.TimeCoordinates.Count}");
            }

            await PlotLine(history.Values.Last());
        }

        private async Task PlotLine(Tag tag)
        {
            SelectedTag = tag;

            await _plotModelHelper.PlotTagOnLinePlotModel(MyPlotModel, SelectedTag, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }
        
        private async void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            MyPlotModel = new PlotModel();
            MyPlotModel = _plotModelHelper.ApplyLinePlotSettings(MyPlotModel, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                await PlotLine(SelectedTag);
            }
        }
    }
}
