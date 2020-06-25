using System.Collections.Generic;
using Prism.Events;

namespace PipelineService
{
    public class PipelineCompletedEvent : PubSubEvent<IList<PipelineCompletedEventModel>>
    {
    }
}
