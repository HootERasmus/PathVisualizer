using Lib.SharedModels;

namespace PipelineService
{
    public class PipelineCompletedEventModel
    {
        public PipelineCompletedEventModel(string name, Tag tag)
        {
            Name = name;
            Tag = tag;
        }

        public string Name { get; }
        public Tag Tag { get; }
    }
}
