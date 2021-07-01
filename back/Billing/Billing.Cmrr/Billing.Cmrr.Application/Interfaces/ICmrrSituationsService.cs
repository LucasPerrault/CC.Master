using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Situation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface ICmrrSituationsService
    {
        Task<CmrrSituation> GetSituationAsync(CmrrSituationFilter situationFilter);
    }

    public class CmrrSituationFilter
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }

        public CmrrAxis Axis { get; set; } = CmrrAxis.Product;

        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<string> DistributorsId { get; set; } = new HashSet<string>();
        public HashSet<BillingStrategy> BillingStrategies { get; set; } = new HashSet<BillingStrategy>();
        public HashSet<string> Sections { get; set; } = new HashSet<string>();
    }
}
