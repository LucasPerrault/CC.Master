using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Cmrr.Domain
{
    public class CmrrSituation
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }

        public CmrrAxis Axis { get; set; }
        public List<CmrrAxisSection> Sections { get; set; }
    }

    public class CmrrAxisSection
    {
        public CmrrAxisSection(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public decimal TotalFrom { get; set; }
        public decimal TotalTo { get; set; }

        public CmrrAmount Upsell { get; set; } = new CmrrAmount();
        public CmrrAmount Creation { get; set; } = new CmrrAmount();
        public CmrrAmount Termination { get; set; } = new CmrrAmount();
        public CmrrAmount Expansion { get; set; } = new CmrrAmount();
        public CmrrAmount Retraction { get; set; } = new CmrrAmount();
    }

    public class CmrrAmount
    {
        public const int TopCount = 10;
        public decimal Amount { get; set; }
        public List<ContractAnalyticSituation> Top { get; set; } = new List<ContractAnalyticSituation>();
    }

    public class ContractAnalyticSituation
    {
        public Breakdown Breakdown { get; }
        public CmrrContractSituation ContractSituation { get; }

        public decimal PartialDiff { get; }

        public ContractAnalyticSituation(Breakdown breakdown, CmrrContractSituation contractSituation)
        {
            Breakdown = breakdown;
            ContractSituation = contractSituation;

            PartialDiff = breakdown.Ratio * ((contractSituation.EndPeriodCount?.EuroTotal ?? 0) - (contractSituation.StartPeriodCount?.EuroTotal ?? 0));
        }
    }

    public class Breakdown
    {
        public AxisSection AxisSection { get; set; }
        public decimal Ratio { get; set; }
    }

    public class AxisSection : ValueObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Id;
                yield return Name;
            }
            set
            {
                EqualityComponents = value;
            }
            
        }
    }
}
