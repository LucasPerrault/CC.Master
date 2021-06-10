using System;
using System.Collections.Generic;
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
                ? (IConditionalQueryableBuilder<T>)new CompareQueryableBuilder<T>(query)
                : new BypassApplyQueryableBuilder<T>(query);

        public static ICompareBoolQueryableBuilder<T> Apply<T>(this IQueryable<T> query, CompareBoolean compareBoolean) =>
            compareBoolean switch
            {
                CompareBoolean.Bypass => new BypassApplyQueryableBuilder<T>(query),
                CompareBoolean.TrueOnly => new CompareQueryableBuilder<T>(query),
                CompareBoolean.FalseOnly => new InverseCompareQueryableBuilder<T>(query),
                _ => throw new ApplicationException($"Could not get querying strategy for CompareBoolean {compareBoolean}")
            };

        public static IConditionalQueryableBuilder<T> WhenNotNullOrEmpty<T>(this IQueryable<T> query, string s) =>
            query.When(!string.IsNullOrEmpty(s));

        public static IConditionalQueryableBuilder<T> WhenNotEmpty<T, U>(this IQueryable<T> query, IReadOnlyCollection<U> collection) =>
            query.When(collection.Any());

        public static IConditionalQueryableBuilder<T> WhenHasValue<T, TStruct>(this IQueryable<T> query, TStruct? nullable)
            where TStruct : struct => query.When(nullable.HasValue);

        public interface IConditionalQueryableBuilder<T>
        {
            IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression);
        }

        public interface ICompareBoolQueryableBuilder<T>
        {
            IQueryable<T> To(Expression<Func<T, bool>> expression);
        }

        private class BypassApplyQueryableBuilder<T> :
            IConditionalQueryableBuilder<T>,
            ICompareBoolQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            public BypassApplyQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression) => _query;
            public IQueryable<T> To(Expression<Func<T, bool>> expression) => ApplyWhere(expression);
        }

        private class CompareQueryableBuilder<T> :
            IConditionalQueryableBuilder<T>,
            ICompareBoolQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            public CompareQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression) => _query.Where(expression);
            public IQueryable<T> To(Expression<Func<T, bool>> expression) => ApplyWhere(expression);
        }

        private class InverseCompareQueryableBuilder<T> :
            IConditionalQueryableBuilder<T>,
            ICompareBoolQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            public InverseCompareQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> ApplyWhere(Expression<Func<T, bool>> expression) => _query.Where(expression.Inverse());
            public IQueryable<T> To(Expression<Func<T, bool>> expression) => ApplyWhere(expression);
        }
    }
}
