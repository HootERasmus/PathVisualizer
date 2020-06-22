using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.SharedModels;

namespace DataLoaderService
{
    public interface IDataLoader
    {
        Task<IList<Tag>> LoadFiles(string[] fileNames);
        List<Tag> Tags { get; }
    }
}
