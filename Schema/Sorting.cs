using HotChocolate.Language;
using HotChocolate.Resolvers;
using System;
using System.Linq;

namespace core_graph_v2.Schema
{
    public static class Sorting
    {
        public static bool HasOrderByArgument(this IResolverContext context, string argumentName = "order")
        {
            try
            {
                var orderByArgument = context.ArgumentLiteral<IValueNode>(argumentName);

                if (orderByArgument != NullValueNode.Default && orderByArgument != null)
                {
                    return true;
                }
            }

            catch
            {
                return false;
            }

            return false;
        }

        public static IQueryable<T> OrderByArgumentOrDefault<T>(this IQueryable<T> query, IResolverContext context, Func<IQueryable<T>> func, string argumentName = "order")
        {
            if (context.HasOrderByArgument(argumentName))
            {
                return query;
            }

            return func.Invoke();
        }
    }
}
