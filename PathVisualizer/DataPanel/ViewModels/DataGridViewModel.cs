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
        public ObservableCollection<DataGridTagModel> Tags { get; set; }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get => _selectedTag;
            set
            {
                if(value == SelectedTag) return;

                _selectedTag = value;
                RaisePropertyChanged();
            }
        }

        public DataGridViewModel(IEventAggregator eventAggregator)
        {
            Tags = new ObservableCollection<DataGridTagModel>();

           var dataEvent = eventAggregator.GetEvent<DataEvent>();
           dataEvent.Subscribe(ShowTags, ThreadOption.UIThread, false, x => x.EventType == Lib.TagEventType.Loaded);

        }

        private void ShowTags(TagEventModel model)
        {
            foreach (var tag in model.Tags)
            {
                Tags.Add(new DataGridTagModel(tag.Id, tag.Count));
            }
        }
    }
}
