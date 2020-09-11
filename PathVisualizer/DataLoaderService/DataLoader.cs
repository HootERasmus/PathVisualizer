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
using Lib.SharedModels.TimeModels;
using Prism.Events;

namespace DataLoaderService
{
    public class DataLoader : IDataLoader
    {
        public List<Tag> Tags { get; }
        private readonly IEventAggregator _eventAggregator;

        public DataLoader(IEventAggregator eventAggregator)
        {
            Tags = new List<Tag>();
            _eventAggregator = eventAggregator;
        }

        public async Task<IList<Tag>> LoadFiles(string[] fileNames)
        {
            return await Task.Run(() =>
            {
                if (fileNames.Any(fileName => !File.Exists(fileName))) return new List<Tag>();

                Tags.Clear();
                try
                {
                    foreach (var fileName in fileNames)
                    {
                        var read = File.ReadAllLines(fileName);

                        var format = read[0].Split(',');
                        if (format.Length > 4)
                        {
                            Progress(DataLoadOldFormat, read, read.Length);
                        }
                        else
                        {
                            Progress(DataLoadNewFormat, read, read.Length);
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    Tags.Clear();
                }

                _eventAggregator.GetEvent<DataEvent>().Publish(new DataEventModel(Tags, TagEventType.Loaded));
                return Tags;
            });
        }

        private void DataLoadOldFormat(string[] oldFormat)
        {
            var id = oldFormat[1];
            var x = double.Parse(oldFormat[2], CultureInfo.InvariantCulture);
            var y = double.Parse(oldFormat[3], CultureInfo.InvariantCulture);
            var z = double.Parse(oldFormat[4], CultureInfo.InvariantCulture);
            var batteryPower = oldFormat[5];
            var timestamp = double.Parse(oldFormat[6], CultureInfo.InvariantCulture);
            var unit = oldFormat[7];
            var dqi = oldFormat[8];

            if (Tags.All(tag => tag.Id != id))
            {
                Tags.Add(new Tag(id, new List<ITimeCoordinate>()));
            }

            Tags.First(tag => tag.Id == id).TimeCoordinates.Add(new UwbTimeCoordinate(x, y, z, batteryPower, timestamp, unit, dqi));
        }

        private void DataLoadNewFormat(string[] newFormat)
        {
            var id = newFormat[0];
            var x = double.Parse(newFormat[1], CultureInfo.InvariantCulture);
            var y = double.Parse(newFormat[2], CultureInfo.InvariantCulture);
            var timestamp = DateTime.Parse(newFormat[3], CultureInfo.InvariantCulture);

            if (Tags.All(tag => tag.Id != id))
            {
                Tags.Add(new Tag(id, new List<ITimeCoordinate>()));
            }

            Tags.First(tag => tag.Id == id).TimeCoordinates.Add(new TimeCoordinate(x, y, timestamp));
           
        }

        private void Progress(Action<string[]> action, string[] lines, int length)
        {
            var count = 1;
            var percentCount = 0;
            var percent = length / 100;
            _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, 100, 0));

            foreach (var line in lines)
            {
                try
                {
                    var splitLine = line.Split(',');
                    action.Invoke(splitLine);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    count++;
                }

                if (count % percent == 0)
                    _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, 100, ++percentCount));
            }
        }
    }
}
