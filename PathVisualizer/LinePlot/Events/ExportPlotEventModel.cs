using Lib.SharedModels;

namespace LinePlot.Events
{
    public class ExportPlotEventModel
    {
        public ExportPlotEventModel(object sender, string path, Tag tag)
        {
            Sender = sender;
            Path = path;
            Tag = tag;
        }

        public object Sender { get; }
        public string Path { get; }
        public Tag Tag { get; }
    }
}
