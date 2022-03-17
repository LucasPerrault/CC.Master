using System.Collections.Generic;

namespace Billing.Cmrr.Infra.Services.Export.Evolution
{
    public class CmrrEvolutionCsvRow
    {
        public string Name { get; set; }
        public string AmountType { get; set; }
        public List<decimal> Amounts { get; set; }
    }
}
