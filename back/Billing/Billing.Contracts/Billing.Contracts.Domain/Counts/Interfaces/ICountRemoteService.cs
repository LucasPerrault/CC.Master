using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using System;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts
{
    public interface ICountContext : IDisposable
    {
        Task<ContractWithCountNumber> GetNumberFromRemoteAsync(Contract contract, AccountingPeriod period);
    }

    public interface ICountRemoteService
    {
        ICountContext GetCountContext();
    }
}
