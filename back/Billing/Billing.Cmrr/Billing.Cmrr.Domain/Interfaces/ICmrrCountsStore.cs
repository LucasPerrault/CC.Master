using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Domain.Interfaces
{
    public interface ICmrrCountsStore
    {
        Task<List<CmrrCount>> GetByPeriodAsync(DateTime period);
    }
}
