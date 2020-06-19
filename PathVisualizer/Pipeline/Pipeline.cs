using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;
using Prism.Events;

namespace Pipeline
{
    public class Pipeline : IPipeline
    {
        private readonly List<List<Func<Tag, Task<Tag>>>> _stages;
        private readonly IEventAggregator _eventAggregator;

        public Pipeline(IEventAggregator eventAggregator)
        {
            _stages = new List<List<Func<Tag, Task<Tag>>>>(5)
            {
                new List<Func<Tag, Task<Tag>>>(),
                new List<Func<Tag, Task<Tag>>>(),
                new List<Func<Tag, Task<Tag>>>(),
                new List<Func<Tag, Task<Tag>>>(),
                new List<Func<Tag, Task<Tag>>>()
            };

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<PipeLineStartEvent>().Subscribe(StartPipeLine);
        }


        public void AddActionToPipe(Func<Tag, Task<Tag>> action, int stage)
        {
            _stages.ElementAt(stage).Add(action);
        }

        private async void StartPipeLine(PipelineStartEventModel model)
        {
            if(model.Tag == null) return;

            var tempTag = model.Tag;

            foreach (var stage in _stages)
            {
                foreach (var action in stage)
                {
                    tempTag = await action(tempTag);
                }
            }

            _eventAggregator.GetEvent<PipelineCompletedEvent>().Publish(tempTag);
        }
    }
}
