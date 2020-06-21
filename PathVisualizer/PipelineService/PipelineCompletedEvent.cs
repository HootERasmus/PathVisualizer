using System.Collections.Generic;
using Lib.SharedModels;
using Prism.Events;

namespace PipelineService
{
    public class PipelineCompletedEvent : PubSubEvent<IDictionary<string, Tag>>
    {
    }
}
