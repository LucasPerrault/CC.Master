using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Tools;

namespace Billing.Contracts.Domain.Counts.Services;

public interface IMissingCountsService
{
    Task<List<MissingCount>> GetAsync(IReadOnlyCollection<Count> countsOverPeriod, AccountingPeriod period);
}

public class MissingCountsService : IMissingCountsService
{
    private readonly IContractsStore _contractsStore;
    private readonly ContractsRightsFilter _contractsRightsFilter;
    private readonly ClaimsPrincipal _principal;

    public MissingCountsService(IContractsStore contractsStore, ContractsRightsFilter contractsRightsFilter, ClaimsPrincipal principal)
    {
        _contractsStore = contractsStore;
        _contractsRightsFilter = contractsRightsFilter;
        _principal = principal;
    }

    public async Task<List<MissingCount>> GetAsync(IReadOnlyCollection<Count> countsOverPeriod, AccountingPeriod period)
    {
        var contractIdsWithCount = countsOverPeriod.Select(c => c.ContractId).ToHashSet();
        var contractIds = await GetContractIdsThatShouldHaveCount(period);

        return contractIds
            .Where(id => !contractIdsWithCount.Contains(id))
            .Select(id => new MissingCount { ContractId = id, Period = period })
            .ToList();
    }

    private async Task<List<int>> GetContractIdsThatShouldHaveCount(AccountingPeriod period)
    {
        var accessRight = await _contractsRightsFilter.GetReadAccessAsync(_principal);
        var filter = new ContractFilter
        {
            StartsOn = CompareDateTime.IsBeforeOrEqual(period),
            TheoreticalEndOn = CompareDateTime.IsAfterOrEqual(period.LastOfMonth()).OrNull(),
            HasEnvironment = CompareBoolean.TrueOnly,
            ArchivedAt = CompareNullableDateTime.IsNull(),
            HasAttachments = CompareBoolean.TrueOnly,
        };

        return await _contractsStore.GetIdsAsync(accessRight, filter);
    }
}
