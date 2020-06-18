using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.SharedModels;

namespace Filters.Models
{
    public interface IFilter
    {
        public Task<List<TimeCoordinate>> Filter(List<TimeCoordinate> originalData);
        public string Name { get; }
    }
}
