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

        public static AccessRight ForDistributorId
            (string distributorId) => new DistributorIdAccessRight(distributorId);

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

    public class DistributorIdAccessRight : AccessRight
    {
        public string DistributorId { get; }
        public DistributorIdAccessRight(string distributorId)
        {
            DistributorId = distributorId;
        }
        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(DistributorIdAccessRight);
                yield return DistributorId;
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
