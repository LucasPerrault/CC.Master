using System.Threading.Tasks;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Services;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Application.Counts;

public class CountsRepository
{
    private readonly IMissingCountsService _missingCountsService;

    public CountsRepository(IMissingCountsService missingCountsService)
    {
        _missingCountsService = missingCountsService;
    }

    public Task<Page<MissingCount>> GetMissingCountsAsync(AccountingPeriod period)
    {
        if (period is null)
        {
            throw new BadRequestException("Month and years query params are mandatory");
        }
        return _missingCountsService.GetAsync(period);
    }
}
