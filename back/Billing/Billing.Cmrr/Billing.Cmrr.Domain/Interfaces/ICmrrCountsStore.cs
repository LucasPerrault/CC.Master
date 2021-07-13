using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Domain.Interfaces
{
    public interface ICmrrCountsStore
    {
        Task<List<CmrrCount>> GetByPeriodsAsync(params DateTime[] period);
        Task<List<CmrrCount>> GetBetweenAsync(DateTime start, DateTime end);
    }
}
