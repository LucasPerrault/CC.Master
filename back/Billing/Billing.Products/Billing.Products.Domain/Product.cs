
using System.Collections.Generic;

namespace Billing.Products.Domain
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string ApplicationCode { get; set; }
        public int? ParentId { get; set; }
        public bool IsEligibleToMinimalBilling { get; set; }
        public bool IsMultiSuite { get; set; }
        public bool IsPromoted { get; set; }
        public string SalesForceCode { get; set; }
        public bool IsFreeUse { get; set; }
        public string DeployRoute { get; set; }

        public int FamilyId { get; set; }
        public ProductFamily Family { get; set; }

        public List<ProductSolution> ProductSolutions { get; set; }

    }

    public class ProductSolution
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }

        public int Share { get; set; }
    }

    public class ProductFamily
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
