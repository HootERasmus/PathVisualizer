namespace Lib.SharedModels
{
    public class ProgressEventModel
    {
        public ProgressEventModel(int minimum, int maximum, int progressValue)
        {
            Maximum = maximum;
            Minimum = minimum;
            ProgressValue = progressValue;
        }

        public int Maximum { get; }
        public int Minimum { get; }
        public int ProgressValue { get; }
    }
}
