using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.SharedModels;
using Lib.SharedModels.TimeModels;

namespace Filters.Models
{
    public interface IFilter
    {
        public Task<List<ITimeCoordinate>> Filter(List<ITimeCoordinate> originalData);
        public string Name { get; }
    }
}
