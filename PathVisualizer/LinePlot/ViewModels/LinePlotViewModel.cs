using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using PipelineService;
using Prism.Events;
using Tag = Lib.SharedModels.Tag;

namespace LinePlot.ViewModels
{
    public class LinePlotViewModel : BindableBase
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
                if(value == BackgroundImage) return;

                _backgroundImage = value;
                RaisePropertyChanged();
            }
        }
        
        public LinePlotViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
        }

        private async void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            await PlotLine(history.Values.Last());
        }

        private async Task PlotLine(Tag tag)
        {
            SelectedTag = tag;

            await PlotModelHelper.PlotTagOnPlotModel(MyPlotModel, SelectedTag, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }
        
        private async void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            BackgroundImage = Settings.BackgroundImage;

            MyPlotModel = new PlotModel();
            MyPlotModel = await PlotModelHelper.ApplySettings(MyPlotModel, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                await PlotLine(SelectedTag);
            }
        }
    }
}
