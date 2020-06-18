namespace Filters.Models
{
    public class MovingAverageFilterModel : IFilter
    {
        public string Name { get; }

        public MovingAverageFilterModel()
        {
            Name = "Moving Average";
        }

        public void Filter()
        {
            throw new System.NotImplementedException();
        }

        
    }
}
