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
    }
}
