using System.Collections.ObjectModel;
using DataPanel.Models;
using Lib.Events;
using Lib.SharedModels;
using Prism.Events;
using Prism.Mvvm;

namespace DataPanel.ViewModels
{
    public class DataGridViewModel : BindableBase
    {
        public ObservableCollection<Tag> Tags { get; set; }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get => _selectedTag;
            set
            {
                if(value == SelectedTag) return;

                _selectedTag = value;
                RaisePropertyChanged();
                _eventAggregator.GetEvent<TagEvent>().Publish(SelectedTag);
            }
        }

        private readonly IEventAggregator _eventAggregator;

        public DataGridViewModel(IEventAggregator eventAggregator)
        {
            Tags = new ObservableCollection<Tag>();

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<DataEvent>().Subscribe(ShowTags, ThreadOption.UIThread, false, x => x.EventType == Lib.TagEventType.Loaded);

        }

        private void ShowTags(DataEventModel model)
        {
            foreach (var tag in model.Tags)
            {
                Tags.Add(tag);
            }
        }
    }
}
