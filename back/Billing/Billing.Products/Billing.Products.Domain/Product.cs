using System.Collections.Generic;
using System.Linq;

namespace Billing.Products.Domain
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int FamilyId { get; set; }
        public ProductFamily Family { get; set; }

        public List<Solution> Solutions => ProductSolutions?.Select(x => x.Solution).ToList();

        public List<ProductSolution> ProductSolutions { get; set; }
    }

    public class ProductSolution
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SolutionId { get; set; }
        public Solution Solution { get; set; }
    }

    public class ProductFamily
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
