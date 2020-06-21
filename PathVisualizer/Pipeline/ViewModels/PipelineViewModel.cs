using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Lib.SharedModels;
using PipelineService;
using Prism.Events;

namespace Pipeline.ViewModels
{
    public class PipelineViewModel : BindableBase
    {
        public ObservableCollection<string> PipelineHistory { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public PipelineViewModel(IEventAggregator eventAggregator)
        {
            PipelineHistory = new ObservableCollection<string>();

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
        }

        private void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            PipelineHistory.Clear();

            foreach (var tag in history)
            {
                PipelineHistory.Add($"{tag.Key} - {tag.Value.TimeCoordinates.Count}");
            }
        }
    }
}
