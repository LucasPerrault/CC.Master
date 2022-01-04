using Billing.Cmrr.Domain.Situation;

namespace Billing.Cmrr.Infra.Services.Export.Situation
{
    internal class CmrrSituationCsvRow
    {
        public string Name { get; set; }
        public decimal TotalFrom { get; set; }
        public decimal TotalTo { get; set; }
        public decimal Variation { get; set; }
        public decimal VariationPercent { get; set; }
        public decimal Creation { get; set; }
        public decimal Upsell { get; set; }
        public decimal Expansion { get; set; }
        public decimal Contraction { get; set; }
        public decimal Termination { get; set; }
        public decimal Nrr { get; set; }
        public decimal ChurnRate { get; set; }

        public CmrrSituationCsvRow(CmrrSubLine line)
            : this(line, line.Name)
        { }

        public CmrrSituationCsvRow(CmrrSubLine line, string name)
        {
            Name = name;
            TotalFrom = ToRound(line.TotalFrom.Amount);
            TotalTo = ToRound(line.TotalTo.Amount);
            Variation = ToRound(TotalTo - TotalFrom);
            VariationPercent = TotalFrom == 0 ? 0 : ToPercentage(TotalTo / TotalFrom) - 100;
            Creation = ToRound(line.Creation.Amount);
            Upsell = ToRound(line.Upsell.Amount);
            Expansion = ToRound(line.Expansion.Amount);
            Contraction = ToRound(line.Contraction.Amount);
            Termination = ToRound(line.Termination.Amount);
            Nrr = GetNetRevenueRetentionRate(TotalFrom, Termination, Contraction, Expansion, Upsell);
            ChurnRate = GetChurnRate(TotalFrom, Termination);
        }

        private static decimal GetNetRevenueRetentionRate(decimal totalFrom, decimal termination, decimal contraction, decimal expansion, decimal upsell)
            => totalFrom == 0
                ? 0
                : ToPercentage((totalFrom + termination + contraction + expansion + upsell) / totalFrom);

        private static decimal GetChurnRate(decimal totalFrom, decimal termination)
            => totalFrom == 0
                ? 0
                : ToPercentage(-termination / totalFrom);

        private static decimal ToPercentage(decimal amount) => decimal.Round(amount * 100, 1);

        private static decimal ToRound(decimal val) => decimal.Round(val, 0);
    }
}
