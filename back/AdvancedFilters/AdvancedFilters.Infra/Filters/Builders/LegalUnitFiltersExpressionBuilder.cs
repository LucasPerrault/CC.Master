using AdvancedFilters.Domain.Instance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders
{
    internal class LegalUnitFiltersExpressionBuilder : AdvancedFiltersExpressionBuilder<LegalUnit, LegalUnitAdvancedCriterion>
    {
        public LegalUnitFiltersExpressionBuilder(LegalUnitAdvancedCriterion criterion)
            : base(criterion)
        { }

        public override IEnumerable<Expression<Func<IEnumerable<LegalUnit>, bool>>> Build()
        {
            yield return Criterion.CountryId.Chain<LegalUnit, int>(legalUnits => legalUnits.Select(i => i.CountryId));
        }
    }
}
