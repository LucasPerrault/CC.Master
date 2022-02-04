using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Tools;

namespace Billing.Contracts.Domain.Counts.Services;

public interface IMissingCountsService
{
    Task<Page<MissingCount>> GetAsync(AccountingPeriod period);
}

public class MissingCountsService : IMissingCountsService
{
    private readonly IContractsStore _contractsStore;
    private readonly ContractsRightsFilter _contractsRightsFilter;
    private readonly ICountsStore _countsStore;
    private readonly ClaimsPrincipal _principal;

    public MissingCountsService(IContractsStore contractsStore, ContractsRightsFilter contractsRightsFilter, ClaimsPrincipal principal, ICountsStore countsStore)
    {
        _contractsStore = contractsStore;
        _contractsRightsFilter = contractsRightsFilter;
        _principal = principal;
        _countsStore = countsStore;
    }

    public async Task<Page<MissingCount>> GetAsync(AccountingPeriod period)
    {
        var counts = await GetCountsAsync(period);
        var contractIdsWithCount = counts.Select(c => c.Contract.Id).ToList();
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
        var filter = new ContractFilter
        {
            StartsOn = CompareDateTime.IsStrictlyBefore(new DateTime(period.Year, period.Month, 15)),
            EndsOn = CompareDateTime.IsStrictlyAfter(new DateTime(period.Year, period.Month, 15)).OrNull(),
            HasEnvironment = CompareBoolean.TrueOnly,
            ArchivedAt = CompareNullableDateTime.IsNull(),
            HasAttachments = CompareBoolean.TrueOnly,
        };

        return await _contractsStore.GetAsync(accessRight, filter);
    }

    private Task<IReadOnlyCollection<Count>> GetCountsAsync(AccountingPeriod period)
    {
        var filter = new CountFilter
        {
            Periods = new HashSet<AccountingPeriod> { period },
        };

        return _countsStore.GetAsync(filter);
    }
}
