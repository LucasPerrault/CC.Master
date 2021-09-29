using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class CompareStringQueryableExtensions
    {
        public static ICompareStringQueryableBuilder<T> Apply<T>(this IQueryable<T> query, CompareString comparison)
        {
            return comparison switch
            {
                BypassCompareString _ => new BypassCompareStringQueryableBuilder<T>(query),
                EqualsCompareString eq => new CompareEqualStringQueryableBuilder<T>(query, eq.Values),
                DoesNotEqualCompareString neq => new CompareNotEqualStringQueryableBuilder<T>(query, neq.Values),
                StartsWithCompareString start => new CompareStartsWithStringQueryableBuilder<T>(query, start.Value),
                _ => throw new ApplicationException("Type of StringCompare not supported"),
            };
        }

        private class BypassCompareStringQueryableBuilder<T> : ICompareStringQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            public BypassCompareStringQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query;
        }

        private class CompareEqualStringQueryableBuilder<T> : ICompareStringQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly HashSet<string> _values;
            public CompareEqualStringQueryableBuilder(IQueryable<T> query, HashSet<string> values)
            {
                _query = query;
                _values = values;
            }

            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query.Where(expression.Chain(s => _values.Contains(s)));
        }

        private class CompareNotEqualStringQueryableBuilder<T> : ICompareStringQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly HashSet<string> _values;
            public CompareNotEqualStringQueryableBuilder(IQueryable<T> query, HashSet<string> values)
            {
                _query = query;
                _values = values;
            }

            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query.Where(expression.Chain(s => !_values.Contains(s)));
        }

        private class CompareStartsWithStringQueryableBuilder<T> : ICompareStringQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly string _value;
            public CompareStartsWithStringQueryableBuilder(IQueryable<T> query, string value)
            {
                _query = query;
                _value = value;
            }

            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query.Where(expression.Chain(s => s.StartsWith(_value)));
        }

        private class CompareContainsStringQueryableBuilder<T> : ICompareStringQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly string _value;
            public CompareContainsStringQueryableBuilder(IQueryable<T> query, string value)
            {
                _query = query;
                _value = value;
            }

            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query.Where(expression.Chain(s => s.Contains(_value)));
        }
    }

    public interface ICompareStringQueryableBuilder<T>
    {
        IQueryable<T> To(Expression<Func<T, string>> expression);
    }
}
