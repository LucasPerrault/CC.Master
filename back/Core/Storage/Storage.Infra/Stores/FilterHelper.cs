using Lucca.Core.Shared.Domain.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Storage.Infra.Stores
{
    public static class FilterHelper
    {
        public static Expression<Func<T, bool>> CombineSafely<T>(this Expression<Func<T, bool>>[] filters)
        {
            if(filters.Length == 0) { return c => true; }

            if(filters.Length == 1) { return filters[0]; }

            return filters.Aggregate((e1, e2) => e1.AndAlso(e2));
        }
    }
}
