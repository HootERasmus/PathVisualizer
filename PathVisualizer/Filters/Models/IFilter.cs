using System;
using System.Collections.Generic;
using System.Text;

namespace Filters.Models
{
    public interface IFilter
    {
        public void Filter();
        public string Name { get; }
    }
}
