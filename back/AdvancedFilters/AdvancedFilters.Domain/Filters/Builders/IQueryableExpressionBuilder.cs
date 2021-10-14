using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AdvancedFilters.Domain.Filters.Builders
{
    public interface IQueryableExpressionBuilder<TValue>
    {
        Expression<Func<IEnumerable<TValue>, bool>> IntersectionOrBypass { get; }
        Expression<Func<TValue, bool>> MatchOrBypass { get; }

        bool CanBuild();
    }
}
