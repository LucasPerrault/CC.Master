namespace Rights.Domain.Filtering
{
    public abstract class AccessRight
    {
        protected internal AccessRight()
        { }

        public static AccessRight All => new AllAccessRight();

        public static AccessRight ForDistributor
            (string distributorId) => new DistributorAccessRight(distributorId);

        public static AccessRight None => new NoAccessRight();
    }

    public class AllAccessRight : AccessRight
    { }

    public class DistributorAccessRight : AccessRight
    {
        public string DistributorCode { get; }
        public DistributorAccessRight(string distributorCode)
        {
            DistributorCode = distributorCode;
        }
    }

    public class NoAccessRight : AccessRight
    { }
}
