using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PipelineService;
using PlotModelService;
using Prism.Events;

namespace HeatMapPlot.ViewModels
{
    public class HeatMapPlotViewModel : BindableBase
    {
        public PlotModel MyPlotModel { get; set; }
        public PlotSettingsEventModel Settings { get; set; }
        public Tag SelectedTag { get; set; }
        
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

        public HeatMapPlotViewModel(IEventAggregator eventAggregator, IPlotModelHelper plotModelHelper)
        {
            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);

            _plotModelHelper = plotModelHelper;
        }

        private async void OnPipelineCompletedEvent(IDictionary<string,Tag> history)
        {
            await PlotHeatMap(history.Values.Last());
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

            MyPlotModel = await _plotModelHelper.ApplyHeatMapPlotSettings(MyPlotModel, Settings);
            
            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                await PlotHeatMap(SelectedTag);
            }
        }
    }
}
