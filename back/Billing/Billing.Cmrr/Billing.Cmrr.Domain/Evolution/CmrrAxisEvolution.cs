using Billing.Cmrr.Domain.Situation;
using System;
using System.Collections.Generic;

namespace Billing.Cmrr.Domain.Evolution
{
    public class CmrrAxisEvolution
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public CmrrAxis Axis { get; set; }
        public List<CmrrEvolutionBreakdownLine> Lines { get; set; }
    }
    public class CmrrEvolutionBreakdownLine : ICmrrEvolutionLine
    {
        public string SectionName { get; set; }
        public DateTime Period { get; set; }
        public decimal Amount { get; set; }
        public decimal Creation { get; set; }
        public decimal Upsell { get; set; }
        public decimal Contraction { get; set; }
        public decimal Expansion { get; set; }
        public decimal Termination { get; set; }

        public CmrrEvolutionBreakdownLine(DateTime period)
        {
            Period = period;
        }
    }
}
