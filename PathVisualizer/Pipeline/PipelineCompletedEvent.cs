using System.Collections.Generic;
using Lib.SharedModels;
using Prism.Events;

namespace Pipeline
{
    public class PipelineCompletedEvent : PubSubEvent<IList<Tag>>
    {
    }
}
