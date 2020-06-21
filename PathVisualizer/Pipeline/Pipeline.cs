using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;
using Prism.Events;

namespace Pipeline
{
    public class Pipeline : IPipeline
    {
        private readonly List<Dictionary<string,Func<Tag, Task<Tag>>>> _stages;
        private readonly IEventAggregator _eventAggregator;

        public Pipeline(IEventAggregator eventAggregator)
        {
            _stages = new List<Dictionary<string, Func<Tag, Task<Tag>>>>(5)
            {
                new Dictionary<string, Func<Tag, Task<Tag>>>(),
                new Dictionary<string, Func<Tag, Task<Tag>>>(),
                new Dictionary<string, Func<Tag, Task<Tag>>>(),
                new Dictionary<string, Func<Tag, Task<Tag>>>(),
                new Dictionary<string, Func<Tag, Task<Tag>>>()
            };

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<PipeLineStartEvent>().Subscribe(StartPipeLine);
        }


        public bool AddActionToPipe(string key, Func<Tag, Task<Tag>> action, int stage)
        {
            try
            {
                _stages.ElementAt(stage).Add(key, action);
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
                _stages.ElementAt(stage).Remove(key);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private async void StartPipeLine(PipelineStartEventModel model)
        {
            if(model.Tag == null) return;

            var history = new List<Tag>();
            
            var tempTag = model.Tag;
            history.Add(tempTag);

            foreach (var stage in _stages)
            {
                foreach (var action in stage)
                {
                    tempTag = await action.Value(tempTag);
                    history.Add(tempTag);
                }
            }

            _eventAggregator.GetEvent<PipelineCompletedEvent>().Publish(history);
        }
    }
}
