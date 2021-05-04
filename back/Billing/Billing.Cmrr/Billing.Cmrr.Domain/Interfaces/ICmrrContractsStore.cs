using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Domain.Interfaces
{
    public interface ICmrrContractsStore
    {
        Task<List<CmrrContract>> GetContractsNotEndedAtAsync(DateTime startPeriod, DateTime endPeriod);
    }
}
