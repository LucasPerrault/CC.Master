using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Instance.Models;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Filters.Builders
{
    public interface IQueryableExpressionBuilderFactory
    {
        IQueryableExpressionBuilder<Environment> Create(EnvironmentAdvancedCriterion criterion);
        IQueryableExpressionBuilder<AppInstance> Create(AppInstanceAdvancedCriterion criterion);
        IQueryableExpressionBuilder<LegalUnit> Create(LegalUnitAdvancedCriterion criterion);
    }
}
