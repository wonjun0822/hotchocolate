using System.Linq;

using core_graph_v2.Data;
using core_graph_v2.Models;

using HotChocolate;
using HotChocolate.Types;

namespace core_graph_v2.Type
{
    public class ActionCmtType : ObjectType<ActionCmt>
    {
        protected override void Configure(IObjectTypeDescriptor<ActionCmt> descriptor)
        {
            descriptor.Description("Comment");

            //descriptor
            //    .Field(p => p.Action)
            //    .ResolveWith<Resolvers>(p => p.GetAction(default!, default!))
            //    .UseDbContext<AppDbContext>()
            //    .Description("Action");
        }

        private class Resolvers
        {
            public Action GetAction(ActionCmt actionCmt, [ScopedService] AppDbContext context)
            {
                return context.Action.FirstOrDefault(p => p.ActionId == actionCmt.ActionId);
            }
        }
    }
}