using System.Linq;
using System.Security.Claims;
using core_graph_v2.Models;
using core_graph_v2.Services;

using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;

using Action = core_graph_v2.Models.Action;

namespace core_graph_v2.Schema
{
    public class Query
    {
        [Authorize]
        public string Test([GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.ToList().Find(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public string Test2()
        {
            return "Test2";
        }

        [UseSorting]
        public IQueryable<Action> GetAction(IResolverContext context, [Service] Service service)
        {
            IQueryable<Action> query = service.GetAction();
            
            return query.OrderByArgumentOrDefault(context, () => query.OrderBy(p => p.ActionId));
        }

        public IQueryable<ActionCmt> GetActionCmt([Service] Service service)
        {
            return service.GetActionCmt();
        }

        [UseSorting]
        public IQueryable<ClashCheck> GetClashSummary(IResolverContext context, [Service] Service service, string jobNo = "")
        {
            IQueryable<ClashCheck> query = service.GetClashSummary(jobNo);

            return query.OrderByArgumentOrDefault(context, () => query);
        }
    }
}