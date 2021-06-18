using System;
using System.Collections.Generic;

namespace Billing.Cmrr.Domain
{
    public class CmrrEvolution
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public List<CmrrEvolutionLine> Lines { get; set; }
    }
    public class CmrrEvolutionLine
    {
        public DateTime Period { get; set; }
        public decimal Amount { get; set; }
        public decimal Upsell { get; set; }
        public decimal Creation { get; set; }
        public decimal Termination { get; set; }
        public decimal Expansion { get; set; }
        public decimal Retraction { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalAmountVariation { get; set; }
        public CmrrEvolutionLine(DateTime period)
        {
            Period = period;
        }
    }
}
