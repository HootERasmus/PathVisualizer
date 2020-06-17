using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lib;
using Lib.Events;
using Lib.SharedModels;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace DataLoader.ViewModels
{
    public class MenuFileOpenViewModel : BindableBase
    {
        public List<Tag> Tags { get; set; }

        public DelegateCommand OpenCommand { get; set; }
        private readonly IEventAggregator _eventAggregator;

        public MenuFileOpenViewModel(IEventAggregator eventAggregator)
        {
            Tags = new List<Tag>();
            OpenCommand = new DelegateCommand(OpenAction);
            _eventAggregator = eventAggregator;
        }

        private async void OpenAction()
        {
            var openFileDialog = new OpenFileDialog { Multiselect = true };

            if (openFileDialog.ShowDialog() != true) return;


            await LoadFiles(openFileDialog.FileNames);
        }

        private async Task LoadFiles(string[] fileNames)
        {

            await Task.Run(() =>
            {
                if (fileNames.Any(fileName => !File.Exists(fileName))) return;

                Tags.Clear();
                try
                {
                    foreach (var fileName in fileNames)
                    {
                        var read = File.ReadAllLines(fileName);

                        foreach (var line in read)
                        {
                            var strings = line.Split(',');

                            var id = strings[1];
                            var x = double.Parse(strings[2], CultureInfo.InvariantCulture);
                            var y = double.Parse(strings[3], CultureInfo.InvariantCulture);
                            var z = double.Parse(strings[4], CultureInfo.InvariantCulture);
                            var batteryPower = strings[5];
                            var timestamp = double.Parse(strings[6], CultureInfo.InvariantCulture);
                            var unit = strings[7];
                            var dqi = strings[8];

                            if (Tags.All(tag => tag.Id != id))
                            {
                                Tags.Add(new Tag(id, new List<TimeCoordinate>()));
                            }
                            Tags.First(tag => tag.Id == id).TimeCoordinates.Add(new TimeCoordinate(x, y, z, batteryPower, timestamp, unit, dqi));
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    Tags.Clear();
                }
                RaisePropertyChanged(nameof(Tags));
            });

            _eventAggregator.GetEvent<DataEvent>().Publish(new TagEventModel(Tags, TagEventType.Loaded));
        }
    }
}
