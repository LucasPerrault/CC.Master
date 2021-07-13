using Billing.Products.Domain;
using System.Collections.Generic;
using Tools;

namespace Billing.Cmrr.Domain.Situation
{
    public class Breakdown : ValueObject
    {
        public AxisSection AxisSection { get; set; }
        public int ProductId { get; set; }
        public decimal Ratio { get; set; }
        public string SubSection { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return AxisSection;
                yield return Ratio;
                yield return SubSection;
                yield return ProductId;
            }
        }
    }

    public class AxisSection : ValueObject
    {
        public int Id { get; }
        public string Name { get; }
        public int Order { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Id;
                yield return Name;
                yield return Order;
            }
        }

        private AxisSection(int id, string name, int order)
        {
            Id = id;
            Name = name;
            Order = order;
        }

        public static AxisSection ForBusinessUnit(BusinessUnit bu) => new AxisSection(bu.Id, bu.Name, bu.DisplayOrder);
        public static AxisSection ForProductFamily(ProductFamily family) => new AxisSection(family.Id, family.Name, family.DisplayOrder);
        public static AxisSection ForSolution(Solution solution) => new AxisSection(solution.Id, solution.Name, solution.BusinessUnit.DisplayOrder);
    }
}
