using System;
using System.Linq.Expressions;

namespace Tools
{
    public static class ExpressionsExtensions
    {

        public static Expression<Func<T, bool>> Inverse<T>(this Expression<Func<T, bool>> e)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(e.Body), e.Parameters[0]);
        }
    }
}
