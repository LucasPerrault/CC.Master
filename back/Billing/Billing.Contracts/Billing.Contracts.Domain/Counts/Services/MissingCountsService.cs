using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using NExtends.Primitives.DateTimes;
using Tools;

namespace Billing.Contracts.Domain.Counts.Services;

public interface IMissingCountsService
{
    Task<Page<MissingCount>> GetAsync(IReadOnlyCollection<Count> countsOverPeriod, AccountingPeriod period);
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

    public async Task<Page<MissingCount>> GetAsync(IReadOnlyCollection<Count> countsOverPeriod, AccountingPeriod period)
    {
        var contractIdsWithCount = countsOverPeriod.Select(c => c.Contract.Id).ToList();
        var contractsShouldBeCount = await GetContractsShouldBeCount(period);
        var contractsWithoutCount = contractsShouldBeCount
            .Where(c => !contractIdsWithCount.Contains(c.Id))
            .ToList();

        var missingCounts = contractsWithoutCount
            .Select(contract => new MissingCount { Contract = contract, Period = period })
            .ToList();

        return new Page<MissingCount> { Count = missingCounts.Count, Items = missingCounts };
    }

    private async Task<List<Contract>> GetContractsShouldBeCount(AccountingPeriod period)
    {
        var accessRight = await _contractsRightsFilter.GetReadAccessAsync(_principal);
        var periodToDate = new DateTime(period.Year, period.Month, 1);
        var filter = new ContractFilter
        {
            StartsOn = CompareDateTime.IsBeforeOrEqual(periodToDate.FirstOfMonth()),
            TheoreticalEndsOn = CompareDateTime.IsAfterOrEqual(periodToDate.LastOfMonth()).OrNull(),
            HasEnvironment = CompareBoolean.TrueOnly,
            ArchivedAt = CompareNullableDateTime.IsNull(),
            HasAttachments = CompareBoolean.TrueOnly,
        };

        return await _contractsStore.GetAsync(accessRight, filter);
    }
}
