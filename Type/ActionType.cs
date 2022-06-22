using System.Linq;

using core_graph_v2.Models;
using core_graph_v2.Services;

using HotChocolate;
using HotChocolate.Types;

namespace core_graph_v2.Type
{
    public class ActionType : ObjectType<Action>
    {
        protected override void Configure(IObjectTypeDescriptor<Action> descriptor)
        {
            descriptor.Description("Action");

            descriptor
                .Field(p => p.ActionNo)
                .ResolveWith<Resolve>(r => r.ActionNo(default!))
                .Description("Tag No.")
                .Authorize();

            descriptor
                .Field(p => p.ActionCmt)
                .ResolveWith<Resolve>(p => p.GetActionCmt(default!, default!))
                .Description("Test");
        }

        private class Resolve
        {
            public string ActionNo([Parent] Action action)
            {
                return action.ActionNo;
            }

            public IQueryable<ActionCmt> GetActionCmt([Parent] Action action, [Service] Service service)
            {
                return service.GetActionCmt().Where(p => p.ActionId == action.ActionId);
            }
        }
    }
}