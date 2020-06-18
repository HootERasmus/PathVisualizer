using Filters.Models;
using Prism.Events;

namespace Filters.Events
{
    public class FilterEvent : PubSubEvent<IFilter>
    {
    }
}
