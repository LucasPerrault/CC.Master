using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Counts.Services;

namespace Billing.Contracts.Application.Counts;

public class CountsRepository
{
    private readonly ICountsStore _countsStore;
    private readonly CountsRightsFilter _rightsFilter;
    private readonly IMissingCountsService _missingCountsService;
    private readonly ClaimsPrincipal _principal;

    public CountsRepository(
        ICountsStore countsStore,
        CountsRightsFilter rightsFilter,
        ClaimsPrincipal principal,
        IMissingCountsService missingCountsService
    )
    {
        _countsStore = countsStore;
        _rightsFilter = rightsFilter;
        _missingCountsService = missingCountsService;
        _principal = principal;
    }

    public async Task<List<MissingCount>> GetMissingCountsAsync(AccountingPeriod period)
    {
        var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
        var filter = new CountFilter { Periods = new HashSet<AccountingPeriod> { period } };

        var counts = await _countsStore.GetAsync(accessRight, filter);
        return await _missingCountsService.GetAsync(counts, period);
    }
}
