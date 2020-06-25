using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lib.SharedModels;
using OxyPlot;
using OxyPlot.Series;
using PipelineService;
using PlotModelService;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SettingsService;

namespace TimePlayer.ViewModels
{
    public class TimePlayerViewModel : BindableBase
    {
        public DateTime CurrentTime => FromUnixTime(CurrentTimeValue);
        public DateTime MaxTime => FromUnixTime(MaximumTimeValue);

        private double _minimumTimeValue;
        public double MinimumTimeValue
        {
            get => _minimumTimeValue;
            set
            {
                if (value == MinimumTimeValue) return;

                _minimumTimeValue = value;
                RaisePropertyChanged();
            }
        }

        private double _currentTimeValue;
        public double CurrentTimeValue
        {
            get => _currentTimeValue;
            set
            {
                if(Math.Abs(value - CurrentTimeValue) < 0.000001) return;

                _currentTimeValue = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentTime));
            }
        }

        private double _maximumTimeValue;
        public double MaximumTimeValue
        {
            get => _maximumTimeValue;
            set
            {
                if (Math.Abs(value - MaximumTimeValue) < 0.000001) return;

                _maximumTimeValue = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MaxTime));
            }
        }

        private string _speed;
        public string Speed
        {
            get => _speed;
            set
            {
                if(value == Speed) return;

                _speed = value;
                RaisePropertyChanged();
            }
        }

        private string _playPauseText;
        public string PlayPauseText
        {
            get => _playPauseText;
            set
            {
                if(value == PlayPauseText) return;

                _playPauseText = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SlowerCommand { get; set; }
        public DelegateCommand PlayPauseCommand { get; set; }
        public DelegateCommand FasterCommand { get; set; }

        public PlotModel MyPlotModel { get; set; }
        //public PlotModel MyDotPlotModel { get; set; }
        public PlotSettingsEventModel Settings { get; set; }
        public Tag SelectedTag { get; set; }
        public ObservableCollection<PipelineCompletedEventModel> PipelineHistory { get; set; }
        private CancellationTokenSource _cts;

        private bool _isPlaying;

        private readonly IPlotModelHelper _plotModelHelper;

        public TimePlayerViewModel(IEventAggregator eventAggregator, IPlotSettingService plotSettingService, IPlotModelHelper plotModelHelper)
        {
            _plotModelHelper = plotModelHelper;
            PipelineHistory = new ObservableCollection<PipelineCompletedEventModel>();
            
            SlowerCommand = new DelegateCommand(SlowerAction, CanPlay);
            PlayPauseCommand = new DelegateCommand(PlayPauseAction, CanPlay);
            FasterCommand = new DelegateCommand(FasterAction, CanPlay);
            PlayPauseText = "Play";
            _isPlaying = false;

            eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);

            Settings = plotSettingService.LoadPlotSettings();
            ApplyPlotSettings(Settings);
        }

        private void SlowerAction()
        {
            
        }

        private void PlayPauseAction()
        {
            if (_isPlaying)
            {
                Pause();
                PlayPauseText = "Play";
                _isPlaying = !_isPlaying;
            }
            else
            {
                Play();
                PlayPauseText = "Pause";
                _isPlaying = !_isPlaying;
            }
        }

        private async void Play()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            var tag = SelectedTag;

            try
            {
                await Task.Run(() =>
                {
                    var scatterSeries = (ScatterSeries) MyPlotModel.Series.ElementAt(1);

                    for (int i = 0; i < tag.TimeCoordinates.Count - 2; i++)
                    {
                        var timeToWaitInSeconds = tag.TimeCoordinates[i+1].Timestamp - tag.TimeCoordinates[i].Timestamp;
                        var timeToWaitInMiliSeconds = timeToWaitInSeconds * 1000;
                        var timeCoordinate = tag.TimeCoordinates[i];
                        var newPoint = new ScatterPoint(timeCoordinate.X, timeCoordinate.Y);
                        
                        scatterSeries.Points.Clear();
                        scatterSeries.Points.Add(newPoint);
                        MyPlotModel.InvalidatePlot(true);
                        CurrentTimeValue = timeCoordinate.Timestamp;

                        token.ThrowIfCancellationRequested();
                        Thread.Sleep((int)timeToWaitInMiliSeconds);
                    }

                }, token);
            }
            catch (Exception )
            {
                Debug.WriteLine("Dot play canceled");
            }
        }

        private void Pause()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
                _cts.Cancel();
            
        }
        
        private void FasterAction()
        {

        }

        private bool CanPlay()
        {
            return SelectedTag != null;
        }

        private async void OnPipelineCompletedEvent(IList<PipelineCompletedEventModel> history)
        {
            PipelineHistory.Clear();

            foreach (var tag in history)
            {
                PipelineHistory.Add(tag);
            }

            if (!history.Any()) return;
            
            SelectedTag = history.Last().Tag;
            await PlotLine(SelectedTag);
            CalculateTime(SelectedTag);

            SlowerCommand.RaiseCanExecuteChanged();
            PlayPauseCommand.RaiseCanExecuteChanged();
            FasterCommand.RaiseCanExecuteChanged();
            Pause();
        }

        private async Task PlotLine(Tag tag)
        {
            await _plotModelHelper.PlotTagOnLinePlotModel(MyPlotModel, SelectedTag, Settings);
            MyPlotModel.Series.Add(new ScatterSeries());

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }

        private async void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            MyPlotModel = new PlotModel();
            MyPlotModel = _plotModelHelper.ApplyLinePlotSettings(MyPlotModel, Settings);
            MyPlotModel.Series.Add(new ScatterSeries());

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                await PlotLine(SelectedTag);
            }
        }

        private void CalculateTime(Tag tag)
        {
            
            MaximumTimeValue = tag.TimeCoordinates.Max(x => x.Timestamp);
            MinimumTimeValue = tag.TimeCoordinates.Min(x => x.Timestamp);
            CurrentTimeValue = MinimumTimeValue;
        }

        public DateTime FromUnixTime(double unixTime)
        {
            return _epoch.AddSeconds(unixTime + Settings.TimeOffSet.TotalSeconds);
        }
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
