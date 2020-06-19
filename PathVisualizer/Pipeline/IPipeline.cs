using System;
using System.Threading.Tasks;
using Lib.SharedModels;

namespace Pipeline
{
    public interface IPipeline
    {
        public void AddActionToPipe(Func<Tag, Task<Tag>> action, int stage);
    }
}
