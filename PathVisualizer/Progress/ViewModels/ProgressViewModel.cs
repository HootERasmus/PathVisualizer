using System.Threading;
using System.Threading.Tasks;
using Prism.Mvvm;
using Lib.Events;
using Lib.SharedModels;
using Prism.Events;

namespace Progress.ViewModels
{
    public class ProgressViewModel : BindableBase
    {
        private int _minimum;
        public int Minimum
        {
            get => _minimum;
            set
            {
                if(value == Minimum) return;

                _minimum = value;
                RaisePropertyChanged();
            }
        }

        private int _maximum;
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value == Maximum) return;

                _maximum = value;
                RaisePropertyChanged();
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (value == ProgressValue) return;

                _progressValue = value;
                ProgressValueString = ProgressValue.ToString();
                RaisePropertyChanged();
            }
        }

        private string _progressValueString;
        public string ProgressValueString
        {
            get => $"{_progressValueString} / {Maximum}";
            set
            {
                if (value == ProgressValueString) return;

                _progressValueString = value;
                RaisePropertyChanged();
            }
        }

        private bool _visibility;
        public bool Visibility
        {
            get => _visibility;
            set
            {
                if(value == _visibility) return;

                _visibility = value;
                RaisePropertyChanged();
            }
        }

        private readonly IEventAggregator _eventAggregator;

        public ProgressViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ProgressEvent>().Subscribe(ProgressUpdate);

            Minimum = 0;
            Maximum = 0;
            ProgressValue = 0;
            Visibility = false;
        }

        private async void ProgressUpdate(ProgressEventModel model)
        {
            await Task.Run(() =>
            {
                Visibility = true;

                Minimum = model.Minimum;
                Maximum = model.Maximum;
                ProgressValue = model.ProgressValue;

                if (Maximum != ProgressValue) return;
                
                Thread.Sleep(1000);
                Visibility = false;

            });
        }
    }
}
