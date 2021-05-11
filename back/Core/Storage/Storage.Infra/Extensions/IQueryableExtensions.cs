using System;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class QueryableExtensions
    {
        public static IConditionalQueryableBuilder<T> When<T>(this IQueryable<T> query, bool condition) =>
            condition
                ? (IConditionalQueryableBuilder<T>)new ConditionalQueryableBuilder<T>(query)
                : new BypassApplyQueryableBuilder<T>(query);

        public interface IConditionalQueryableBuilder<T>
        {
            IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression);
        }

        public static IConditionalQueryableBuilder<T> WhenNotBoth<T>(this IQueryable<T> query, BoolCombination boolCombination) =>
            query.When(boolCombination != BoolCombination.Both);

        public static IConditionalQueryableBuilder<T> WhenNotNullOrEmpty<T>(this IQueryable<T> query, string s) =>
            query.When(!string.IsNullOrEmpty(s));

        public static IConditionalQueryableBuilder<T> WhenHasValue<T, U>(this IQueryable<T> query, U? nullable) where U : struct =>
            query.When(nullable.HasValue);

        private class BypassApplyQueryableBuilder<T> : IConditionalQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;

            public BypassApplyQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression) => _query;
        }

        private class ConditionalQueryableBuilder<T> : IConditionalQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;

            public ConditionalQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression) => _query.Where(expression);
        }
    }
}
