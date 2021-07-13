using System.Collections.Generic;

namespace Billing.Products.Domain
{
    public class BusinessUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Solution> Solutions { get; set; }
        public int DisplayOrder { get; set; }
    }
}
