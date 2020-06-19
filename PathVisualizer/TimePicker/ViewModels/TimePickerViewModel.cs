using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
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

        private readonly IEventAggregator _eventAggregator;

        public TimePickerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TagSelectionEvent>().Subscribe(CalculateTime);
        }

        private void CalculateTime(Tag tag)
        {
            MaximumTime = tag.TimeCoordinates.Max(x => x.Timestamp);
            MinimumTime = tag.TimeCoordinates.Min(x => x.Timestamp);

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
