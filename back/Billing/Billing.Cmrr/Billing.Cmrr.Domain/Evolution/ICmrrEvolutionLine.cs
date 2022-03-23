using System;

namespace Billing.Cmrr.Domain.Evolution
{
    public interface ICmrrEvolutionLine
    {
        DateTime Period { get; set; }
        decimal Amount { get; set; }
        decimal Upsell { get; set; }
        decimal Creation { get; set; }
        decimal Termination { get; set; }
        decimal Expansion { get; set; }
        decimal Contraction { get; set; }
    }
}
