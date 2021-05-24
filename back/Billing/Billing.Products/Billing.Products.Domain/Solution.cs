using System.Collections.Generic;

namespace Billing.Products.Domain
{
    public class Solution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int ParentId { get; set; }
        public bool IsContactNeeded { get; set; }

        public int BusinessUnitId { get; set; }
        public BusinessUnit BusinessUnit { get; set; }
        public List<ProductSolution> ProductSolutions { get; set; }
    }
}
