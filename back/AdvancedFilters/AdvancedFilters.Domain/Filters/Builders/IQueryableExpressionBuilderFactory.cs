using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Instance.Models;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Filters.Builders
{
    public interface IQueryableExpressionBuilderFactory
    {
        IQueryableExpressionBuilder<Environment> Create(EnvironmentAdvancedCriterion criterion);
        IQueryableExpressionBuilder<Establishment> Create(EstablishmentAdvancedCriterion criterion);
        IQueryableExpressionBuilder<AppInstance> Create(AppInstanceAdvancedCriterion criterion);
        IQueryableExpressionBuilder<LegalUnit> Create(LegalUnitAdvancedCriterion criterion);
        IQueryableExpressionBuilder<Distributor> Create(DistributorAdvancedCriterion distributorAdvancedCriterion);
        IQueryableExpressionBuilder<AppContact> Create(AppContactAdvancedCriterion criterion);
        IQueryableExpressionBuilder<ClientContact> Create(ClientContactAdvancedCriterion criterion);
        IQueryableExpressionBuilder<SpecializedContact> Create(SpecializedContactAdvancedCriterion criterion);
        IQueryableExpressionBuilder<Contract> Create(ContractAdvancedCriterion criterion);
        IQueryableExpressionBuilder<Client> Create(ClientAdvancedCriterion criterion);
    }
}
