using Lucca.Core.Shared.Domain.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Storage.Infra.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> CombineSafelyAnd<T>(this Expression<Func<T, bool>>[] filters)
        {
            return filters.Length switch
            {
                0 => c => true,
                1 => filters[0],
                _ => filters.Aggregate((e1, e2) => e1.AndAlso(e2))
            };
        }

        public static Expression<Func<T, bool>> CombineSafelyOr<T>(this Expression<Func<T, bool>>[] filters)
        {
            return filters.Length switch
            {
                0 => c => false,
                1 => filters[0],
                _ => filters.Aggregate((e1, e2) => e1.SmartOrElse(e2))
            };
        }

        internal static Expression<Func<TIn, TOut>> Chain<TIn, TInterstitial, TOut>(
            this Expression<Func<TIn, TInterstitial>> inner,
            Expression<Func<TInterstitial, TOut>> outer)
        {
            var visitor = new SwapVisitor(outer.Parameters[0], inner.Body);
            return Expression.Lambda<Func<TIn, TOut>>(visitor.Visit(outer.Body), inner.Parameters);
        }

        private class SwapVisitor : ExpressionVisitor
        {
            private readonly Expression _source, _replacement;

            public SwapVisitor(Expression source, Expression replacement)
            {
                _source = source;
                _replacement = replacement;
            }

            public override Expression Visit(Expression node)
            {
                return node == _source ? _replacement : base.Visit(node);
            }
        }
    }
}
