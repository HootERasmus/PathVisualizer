using System;

namespace Filters.Models
{
    public class RemoveDeltaFilterModel : IFilter
    {
        public string Name { get; }

        public RemoveDeltaFilterModel()
        {
            Name = "Remove Delta";
        }

        public void Filter()
        {
            throw new NotImplementedException();
        }
    }
}
