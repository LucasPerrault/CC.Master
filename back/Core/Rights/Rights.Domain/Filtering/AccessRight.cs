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
            (int distributorId) => new DistributorAccessRight(distributorId);

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

    public class DistributorAccessRight : AccessRight
    {
        public int DistributorId { get; }
        public DistributorAccessRight(int distributorId)
        {
            DistributorId = distributorId;
        }
        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(DistributorAccessRight);
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
