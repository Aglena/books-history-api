using System.Linq.Expressions;

namespace BookHistoryApi.Application.Querying
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> q, int page, int pageSize)
        { 
            return q.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> ApplyOrder<T, TKey>(
            this IQueryable<T> q,
            Expression<Func<T, TKey>> key,
            bool asc,
            bool thenBy = false)
        {
            if (q is IOrderedQueryable<T> ordered && thenBy)
                return asc ? ordered.ThenBy(key) : ordered.ThenByDescending(key);

            return asc ? q.OrderBy(key) : q.OrderByDescending(key);
        }

    }
}
