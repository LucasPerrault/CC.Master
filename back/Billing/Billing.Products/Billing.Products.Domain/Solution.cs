using System.Collections.Generic;

namespace Billing.Products.Domain
{
    public class Solution
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductSolution> ProductSolutions { get; set; }
    }

    public class SolutionFamily
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BusinessUnitId { get; set; }
    }

    public class BusinessUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
