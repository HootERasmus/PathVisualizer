using System.Collections.Generic;

namespace Lib.SharedModels
{
    public class TagEventModel
    {
        public TagEventModel(List<Tag> tags, TagEventType eventType)
        {
            Tags = tags;
            EventType = eventType;
        }

        public List<Tag> Tags { get; set; }
        public TagEventType EventType { get; set; }
    }
}
