using Billing.Cmrr.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface ICmrrSituationsService
    {
        Task<List<CmrrContratSituation>> GetContractSituationsAsync(DateTime startPeriod, DateTime endPeriod);
    }
}
