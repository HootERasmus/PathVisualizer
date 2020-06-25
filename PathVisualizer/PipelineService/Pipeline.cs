using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;
using Prism.Events;

namespace PipelineService
{
    public class Pipeline : IPipeline
    {
        private readonly List<List<KeyValuePair<string,Func<Tag, Task<Tag>>>>> _stages;
        private readonly IEventAggregator _eventAggregator;

        public Pipeline(IEventAggregator eventAggregator)
        {
            _stages = new List<List<KeyValuePair<string, Func<Tag, Task<Tag>>>>>(5)
            {
                new List<KeyValuePair<string,Func<Tag, Task<Tag>>>>(),
                new List<KeyValuePair<string,Func<Tag, Task<Tag>>>>(),
                new List<KeyValuePair<string,Func<Tag, Task<Tag>>>>(),
                new List<KeyValuePair<string,Func<Tag, Task<Tag>>>>(),
                new List<KeyValuePair<string,Func<Tag, Task<Tag>>>>(),
            };

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<PipelineStartEvent>().Subscribe(StartPipeLine);
        }

        public bool AddActionToPipe(string key, Func<Tag, Task<Tag>> action, int stage)
        {
            try
            {
                var pair = _stages.ElementAt(stage).FirstOrDefault(x => x.Key == key);
                if (pair.Key == default)
                    _stages.ElementAt(stage).Add(new KeyValuePair<string, Func<Tag, Task<Tag>>>(key, action));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool RemoveActionFromPipe(string key, int stage)
        {
            try
            {
                var pair = _stages.ElementAt(stage).FirstOrDefault(x => x.Key == key);
                if(pair.Key != default)
                    _stages.ElementAt(stage).Remove(pair);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<IList<PipelineCompletedEventModel>> StartSilentPipeline(PipelineStartEventModel model)
        {
            if (model.Tag == null) return new List<PipelineCompletedEventModel>();

            var history = new List<PipelineCompletedEventModel>();

            var tempTag = model.Tag;
            history.Add(new PipelineCompletedEventModel("Original", tempTag));

            foreach (var stage in _stages)
            {
                foreach (var action in stage)
                {
                    tempTag = await action.Value(tempTag);
                    history.Add(new PipelineCompletedEventModel(action.Key, tempTag));
                }
            }

            return history;
        }

        private async void StartPipeLine(PipelineStartEventModel model)
        {

            var history = await StartSilentPipeline(model);
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Publish(history);
        }
    }
}
