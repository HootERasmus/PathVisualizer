using System;
using System.Threading.Tasks;
using Lib.SharedModels;

namespace Pipeline
{
    public interface IPipeline
    {
        public bool AddActionToPipe(string key, Func<Tag, Task<Tag>> action, int stage);
        public bool RemoveActionFromPipe(string key, int stage);
    }
}
