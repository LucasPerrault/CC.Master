using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class AppContactCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<AppContact, AppContactAdvancedCriterion>
    {
        public AppContactCriterionExpressionBuilder(AppContactAdvancedCriterion criterion, IAdvancedExpressionChainer chainer)
            : base(criterion, chainer)
        { }

        protected override IEnumerable<IPropertyExpressionBuilder<AppContact>> GetCriteria()
        {
            yield return Apply(Criterion.IsConfirmed).To(c => c.IsConfirmed);
            yield return Apply(Criterion.AppInstance).To(c => c.AppInstance);
            yield return Apply(Criterion.Environment).To(c => c.Environment);
            yield return Apply(Criterion.LegalUnit).To(c => c.Establishment.LegalUnit);
        }
    }
}
