using System.Collections.Generic;
using Tools;

namespace Rights.Domain.Filtering
{
    public abstract class AccessRight : ValueObject
    {
        protected internal AccessRight()
        { }

        public static AccessRight All => new AllAccessRight();

        public static AccessRight ForDistributor
            (string distributorCode) => new DistributorCodeAccessRight(distributorCode);

        public static AccessRight None => new NoAccessRight();
    }

    public class AllAccessRight : AccessRight
    {
        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(AllAccessRight);
            }
        }
    }

    public class DistributorCodeAccessRight : AccessRight
    {
        public string DistributorCode { get; }
        public DistributorCodeAccessRight(string distributorCode)
        {
            DistributorCode = distributorCode;
        }
        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(DistributorCodeAccessRight);
                yield return DistributorCode;
            }
        }
    }

    public class NoAccessRight : AccessRight
    {

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(NoAccessRight);
            }
        }
    }
}
