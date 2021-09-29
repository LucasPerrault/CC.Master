using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class CompareIntQueryableExtensions
    {
        public static ICompareIntQueryableBuilder<T> Apply<T>(this IQueryable<T> query, CompareInt comparison)
        {
            return comparison.Type switch
            {
                CompareIntType.Bypass => new BypassCompareIntQueryableBuilder<T>(query),
                CompareIntType.Equals => new CompareEqualIntQueryableBuilder<T>(query, comparison.Value),
                CompareIntType.DoesNotEqual => new CompareNotEqualIntQueryableBuilder<T>(query, comparison.Value),
                _ => throw new InvalidEnumArgumentException(nameof(comparison.Type), (int)comparison.Type, typeof(CompareIntType)),
            };
        }

        private class BypassCompareIntQueryableBuilder<T> : ICompareIntQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            public BypassCompareIntQueryableBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> To(Expression<Func<T, int>> expression) => _query;
        }

        private class CompareEqualIntQueryableBuilder<T> : ICompareIntQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly int _value;
            public CompareEqualIntQueryableBuilder(IQueryable<T> query, int value)
            {
                _query = query;
                _value = value;
            }

            public IQueryable<T> To(Expression<Func<T, int>> expression) => _query.Where(expression.Chain(s => s == _value));
        }

        private class CompareNotEqualIntQueryableBuilder<T> : ICompareIntQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly int _value;
            public CompareNotEqualIntQueryableBuilder(IQueryable<T> query, int value)
            {
                _query = query;
                _value = value;
            }

            public IQueryable<T> To(Expression<Func<T, int>> expression) => _query.Where(expression.Chain(s => s != _value));
        }
    }

    public interface ICompareIntQueryableBuilder<T>
    {
        IQueryable<T> To(Expression<Func<T, int>> expression);
    }
}
