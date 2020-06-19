using Lib.SharedModels;
using Prism.Events;

namespace Pipeline
{
    public class PipeLineStartEvent : PubSubEvent<PipelineStartEventModel>
    {
    }

    public class PipelineStartEventModel
    {
        public PipelineStartEventModel(object sender, Tag tag)
        {
            Sender = sender;
            Tag = tag;
        }

        public Tag Tag { get; }
        public object Sender { get; }
    }
}
