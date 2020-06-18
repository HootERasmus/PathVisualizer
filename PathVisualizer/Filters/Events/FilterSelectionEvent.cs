using Filters.Models;
using Prism.Events;

namespace Filters.Events
{
    public class FilterSelectionEvent : PubSubEvent<IFilter>
    {
    }
}
