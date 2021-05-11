using System;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class QueryableExtensions
    {
        internal static Expression<Func<T, bool>> Inverse<T>(this Expression<Func<T, bool>> e)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(e.Body), e.Parameters[0]);
        }

        public static IConditionalQueryableBuilder<T> When<T>(this IQueryable<T> query, bool condition) =>
            condition
                ? (IConditionalQueryableBuilder<T>)new ConditionalQueryableBuilder<T>(query)
                : new BypassApplyQueryableBuilder<T>(query);

        public static IConditionalQueryableBuilder<T> When<T>(this IQueryable<T> query, BoolCombination boolCombination) =>
            boolCombination switch
            {
                BoolCombination.Both => new BypassApplyQueryableBuilder<T>(query),
                BoolCombination.TrueOnly => new ConditionalQueryableBuilder<T>(query),
                BoolCombination.FalseOnly => new InverseConditionalQueryableBuilder<T>(query),
                _ => throw new ApplicationException($"Could not get querying strategy for boolCombination {boolCombination}")
            };

        public static IConditionalQueryableBuilder<T> WhenNotNullOrEmpty<T>(this IQueryable<T> query, string s) =>
            query.When(!string.IsNullOrEmpty(s));

        public static IConditionalQueryableBuilder<T> WhenHasValue<T, TStruct>(this IQueryable<T> query, TStruct? nullable)
            where TStruct : struct => query.When(nullable.HasValue);

        public interface IConditionalQueryableBuilder<T>
        {
            IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression);
        }

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

        private class InverseConditionalQueryableBuilder<T> : IConditionalQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            public InverseConditionalQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression) => _query.Where(expression.Inverse());
        }
    }
}
