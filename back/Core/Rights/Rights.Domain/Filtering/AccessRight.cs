namespace Rights.Domain.Filtering
{
    public abstract class AccessRight
    {
        protected internal AccessRight()
        { }

        public static AccessRight All => new AllAccessRight();

        public static AccessRight ForDistributor
            (string distributorCode) => new DistributorCodeAccessRight(distributorCode);

        public static AccessRight None => new NoAccessRight();
    }

    public class AllAccessRight : AccessRight
    { }

    public class DistributorCodeAccessRight : AccessRight
    {
        public string DistributorCode { get; }
        public DistributorCodeAccessRight(string distributorCode)
        {
            DistributorCode = distributorCode;
        }
    }

    public class NoAccessRight : AccessRight
    { }
}
