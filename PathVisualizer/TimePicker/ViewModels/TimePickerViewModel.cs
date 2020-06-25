using Prism.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using PipelineService;
using Prism.Commands;
using Prism.Events;

namespace TimePicker.ViewModels
{
    public class TimePickerViewModel : BindableBase
    {
        private double _minimumTime;
        public double MinimumTime
        {
            get => _minimumTime;
            set
            {
                if (Math.Abs(value - MinimumTime) < 0.000001) return;
                _minimumTime = value;

                RaisePropertyChanged();
            }
        }

        private double _maximumTime;
        public double MaximumTime
        {
            get => _maximumTime;
            set
            {
                if (Math.Abs(value - MaximumTime) < 0.000001) return;
                _maximumTime = value;

                RaisePropertyChanged();
            }
        }

        private double _upperTimeValue;
        public double UpperTimeValue
        {
            get => _upperTimeValue;
            set
            {
                if (Math.Abs(value - UpperTimeValue) < 0.000001) return;
                _upperTimeValue = value;

                RaisePropertyChanged();
                UpperTimeValueDateTime = FromUnixTime(UpperTimeValue);

                if (FreezeTimeWindow)
                    LowerTimeValue = value - _timeWindow;
            }
        }

        private double _lowerTimeValue;
        public double LowerTimeValue
        {
            get => _lowerTimeValue;
            set
            {
                if (Math.Abs(value - LowerTimeValue) < 0.000001) return;
                _lowerTimeValue = value;

                RaisePropertyChanged();
                LowerTimeValueDateTime = FromUnixTime(LowerTimeValue);

                if (FreezeTimeWindow)
                    UpperTimeValue = value + _timeWindow;
            }
        }

        private DateTime _upperTimeValueDateTime;
        public DateTime UpperTimeValueDateTime
        {
            get => _upperTimeValueDateTime;
            set
            {
                if (value == UpperTimeValueDateTime) return;
                _upperTimeValueDateTime = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _lowerTimeValueDateTime;
        public DateTime LowerTimeValueDateTime
        {
            get => _lowerTimeValueDateTime;
            set
            {
                if (value == LowerTimeValueDateTime) return;
                _lowerTimeValueDateTime = value;

                RaisePropertyChanged();
            }
        }

        private bool _freezeTimeWindow;
        public bool FreezeTimeWindow
        {
            get => _freezeTimeWindow;
            set
            {
                if (value == FreezeTimeWindow) return;

                _freezeTimeWindow = value;
                _timeWindow = UpperTimeValue - LowerTimeValue;
                RaisePropertyChanged();
            }
        }
        private double _timeWindow;

        private readonly IEventAggregator _eventAggregator;
        private Tag _lastTag;


        public DelegateCommand MouseButtonUpCommand { get; set; }

        public TimePickerViewModel(IEventAggregator eventAggregator, IPipeline pipeline)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TagSelectionEvent>().Subscribe(CalculateTime);

            MouseButtonUpCommand = new DelegateCommand(MouseButtonUpAction);

            pipeline.AddActionToPipe(nameof(CutTimeFromTag),CutTimeFromTag, 1);
        }

        private Task<Tag> CutTimeFromTag(Tag tag)
        {
            return Task.Run(() => {
                var timeCoordinates = tag.TimeCoordinates.Where(x => x.Timestamp >= LowerTimeValue && x.Timestamp <= UpperTimeValue).ToList();
                return new Tag(tag.Id, timeCoordinates);
            });
        }

        public void MouseButtonUpAction()
        {
            _eventAggregator.GetEvent<PipelineStartEvent>().Publish(new PipelineStartEventModel(this, _lastTag));
        }

        private void CalculateTime(Tag tag)
        {
            if(tag == null) return;
            
            _lastTag = tag;
            
            MaximumTime = _lastTag.TimeCoordinates.Max(x => x.Timestamp);
            MinimumTime = _lastTag.TimeCoordinates.Min(x => x.Timestamp);

            UpperTimeValue = MaximumTime;
            LowerTimeValue = MinimumTime;
        }

        public static DateTime FromUnixTime(double unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}