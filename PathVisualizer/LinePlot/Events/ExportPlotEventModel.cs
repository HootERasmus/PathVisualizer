namespace LinePlot.Events
{
    public class ExportPlotEventModel
    {
        public ExportPlotEventModel(object sender, string path)
        {
            Sender = sender;
            Path = path;
        }

        public object Sender { get; }
        public string Path { get; }
    }
}
