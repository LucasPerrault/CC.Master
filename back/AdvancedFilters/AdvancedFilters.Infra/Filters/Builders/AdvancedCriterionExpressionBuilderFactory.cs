using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Implementations;

namespace AdvancedFilters.Infra.Filters.Builders
{
    internal static class QueryableExpressionBuilderExtensions
    {
        public static IQueryableExpressionBuilder<TValue> GetExpressionBuilder<TValue>(this AdvancedCriterion<TValue> criterion)
        {
            var factory = new AdvancedCriterionExpressionBuilderFactory();

            return criterion?.GetExpressionBuilder(factory)
                ?? new BypassExpressionBuilder<TValue>();
        }
    }

    internal class AdvancedCriterionExpressionBuilderFactory : IQueryableExpressionBuilderFactory
    {
        public IQueryableExpressionBuilder<Environment> Create(EnvironmentAdvancedCriterion criterion)
        {
            return new EnvironmentCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<Establishment> Create(EstablishmentAdvancedCriterion criterion)
        {
            return new EstablishmentCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<AppInstance> Create(AppInstanceAdvancedCriterion criterion)
        {
            return new AppInstanceCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<LegalUnit> Create(LegalUnitAdvancedCriterion criterion)
        {
            return new LegalUnitCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<Distributor> Create(DistributorAdvancedCriterion criterion)
        {
            return new DistributorCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<AppContact> Create(AppContactAdvancedCriterion criterion)
        {
            return new AppContactCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<ClientContact> Create(ClientContactAdvancedCriterion criterion)
        {
            return new ClientContactCriterionExpressionBuilder(criterion);
        }

        public IQueryableExpressionBuilder<SpecializedContact> Create(SpecializedContactAdvancedCriterion criterion)
        {
            return new SpecializedContactCriterionExpressionBuilder(criterion);
        }
    }
}
