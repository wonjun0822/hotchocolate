using core_graph_v2.Models;

using HotChocolate;
using HotChocolate.Types;

namespace core_graph_v2.Schema
{
    public class SubScription
    {
        [Subscribe]
        [Topic]
        public Action ActionAdded([EventMessage] Action action) => action;
    }
}
