using Lucca.Core.Shared.Domain.Expressions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Storage.Infra.Extensions
{
    internal static class CompareDateTimeExtensions
    {
        public static Expression<Func<DateTime, bool>> GetExpression(this CompareDateTime comparison)
        {
            return comparison switch
            {
                BypassCompareDateTime _ => _ => true,
                IsAfterCompareDateTime { IsStrict: true } isAfter => d => d > isAfter.Value,
                IsAfterCompareDateTime { IsStrict: false } isAfter => d => d >= isAfter.Value,
                IsBeforeCompareDateTime { IsStrict: true } isBefore => d => d < isBefore.Value,
                IsBeforeCompareDateTime { IsStrict: false } isBefore => d => d <= isBefore.Value,
                IsBetweenCompareDateTime { IsStrict: true } isBetween => d => d < isBetween.Max && d > isBetween.Min,
                IsBetweenCompareDateTime { IsStrict: false } isBetween => d => d <= isBetween.Max && d >= isBetween.Min,
                IsEqualCompareDateTime isEqual => d => d == isEqual.Value,
                _ => throw new InvalidEnumArgumentException(nameof(comparison)),
            };
        }
    }

    public static class CompareDateTimeQueryableExtensions
    {
        public static ICompareDateTimeQueryableBuilder<T> Apply<T>(this IQueryable<T> query, CompareDateTime comparison)
        {
            return new CompareDateTimeQueryableBuilder<T>(query, comparison.GetExpression());
        }

        public interface ICompareDateTimeQueryableBuilder<T>
        {
            IQueryable<T> To(Expression<Func<T, DateTime>> expression);
        }


        internal class CompareDateTimeQueryableBuilder<T> : ICompareDateTimeQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private Expression<Func<DateTime, bool>> _expression { get; }

            public CompareDateTimeQueryableBuilder(IQueryable<T> query, Expression<Func<DateTime, bool>> expression)
            {
                _query = query;
                _expression = expression;
            }

            public IQueryable<T> To(Expression<Func<T, DateTime>> expression)
            {
                return _query.Where(expression.Chain(_expression));
            }
        }
    }

    public static class CompareNullableDateTimeQueryableExtensions
    {
        public static ICompareNullableDateTimeQueryableBuilder<T> Apply<T>(this IQueryable<T> query, CompareNullableDateTime comparison)
        {
            return comparison switch
            {
                OrNullCompareNullableDatetime orNull => new OrNullCompareNullableDatetimeQueryableBuilder<T>(query, orNull.CompareDateTime),
                AndNotNullCompareNullableDateTime hasValue => new AndNotNullCompareNullableDateTimeQueryBuilder<T>(query, hasValue.CompareDateTime),
                IsNullCompareNullableDateTimeQueryBuilder _ => new IsNullCompareNullableDateTimeQueryBuilder<T>(query),
                _ => throw new InvalidEnumArgumentException(nameof(comparison)),
            };
        }

        public interface ICompareNullableDateTimeQueryableBuilder<T>
        {
            IQueryable<T> To(Expression<Func<T, DateTime?>> expression);
        }

        private class OrNullCompareNullableDatetimeQueryableBuilder<T> : ICompareNullableDateTimeQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly CompareDateTime _compareDateTime;

            public OrNullCompareNullableDatetimeQueryableBuilder(IQueryable<T> query, CompareDateTime compareDateTime)
            {
                _query = query;
                _compareDateTime = compareDateTime;
            }

            public IQueryable<T> To(Expression<Func<T, DateTime?>> expression) =>
                _query.Where(expression.Chain(d => !d.HasValue)
                    .OrElse(expression.Chain(d => d.Value).Chain(_compareDateTime.GetExpression()))
                );
        }

        private class IsNullCompareNullableDateTimeQueryBuilder<T> : ICompareNullableDateTimeQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;

            public IsNullCompareNullableDateTimeQueryBuilder(IQueryable<T> query) => _query = query;
            public IQueryable<T> To(Expression<Func<T, DateTime?>> expression) => _query.Where(expression.Chain(d => !d.HasValue));
        }

        private class AndNotNullCompareNullableDateTimeQueryBuilder<T> : ICompareNullableDateTimeQueryableBuilder<T>
        {
            private readonly IQueryable<T> _query;
            private readonly CompareDateTime _compareDateTime;

            public AndNotNullCompareNullableDateTimeQueryBuilder(IQueryable<T> query, CompareDateTime compareDateTime)
            {
                _query = query;
                _compareDateTime = compareDateTime;
            }

            public IQueryable<T> To(Expression<Func<T, DateTime?>> expression) =>
                _query.Where(expression.Chain(d => d.HasValue)
                    .AndAlso(expression.Chain(d => d.Value).Chain(_compareDateTime.GetExpression()))
                );
        }
    }
}
