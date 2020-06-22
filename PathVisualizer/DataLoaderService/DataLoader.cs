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

                _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, fileNames.Length, 0));
                var count = 0;

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

                        _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, fileNames.Length, ++count));
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
    }
}
