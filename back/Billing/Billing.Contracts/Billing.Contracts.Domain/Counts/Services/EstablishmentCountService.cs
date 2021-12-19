using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Environments;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Counts
{
    public class EstablishmentCountService
    {
        public IEnumerable<EstablishmentAttachment> GetForPeriod(Contract contract, AccountingPeriod period)
        {
            return contract.Attachments
                .Where(a => a.StartsOn <= period)
                .Where(a => !a.EndsOn.HasValue || a.EndsOn > period);
        }

        public List<CountDetail> FilterFreeMonths(Contract contract, List<CountDetail> details, AccountingPeriod p)
        {
            var attachmentsStartOnPerId = contract.Attachments.ToDictionary(a => a.EstablishmentRemoteId, a => a.StartsOn.AddMonths(a.NumberOfFreeMonths));
            return details.Where(d => p >= attachmentsStartOnPerId[d.EstablishmentId]).ToList();
        }
    }
}
