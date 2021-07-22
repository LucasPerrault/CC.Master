using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class CompareStringQueryableExtensions
    {
        public static ICompareStringQueryableBuilder<T> Apply<T>(this IQueryable<T> query, CompareString comparison)
        {
            return comparison.Type switch
            {
                CompareStringType.Bypass => new BypassCompareStringQueryableBuilder<T>(query),
                CompareStringType.Equals => new CompareEqualStringQueryableBuilder<T>(query, comparison.Value),
                CompareStringType.DoesNotEqual => new CompareNotEqualStringQueryableBuilder<T>(query, comparison.Value),
                CompareStringType.StartsWith => new CompareStartsWithStringQueryableBuilder<T>(query, comparison.Value),
                _ => throw new InvalidEnumArgumentException(nameof(comparison.Type), (int)comparison.Type, typeof(CompareStringType)),
            };
        }

        public interface ICompareStringQueryableBuilder<T>
        {
            IQueryable<T> To(Expression<Func<T, string>> expression);
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
            private readonly string _value;
            public CompareEqualStringQueryableBuilder(IQueryable<T> query, string value)
            {
                _query = query;
                _value = value;
            }

            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query.Where(expression.Chain(s => s == _value));
        }

        private class CompareNotEqualStringQueryableBuilder<T> : ICompareStringQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly string _value;
            public CompareNotEqualStringQueryableBuilder(IQueryable<T> query, string value)
            {
                _query = query;
                _value = value;
            }

            public IQueryable<T> To(Expression<Func<T, string>> expression) => _query.Where(expression.Chain(s => s != _value));
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
    }
}
