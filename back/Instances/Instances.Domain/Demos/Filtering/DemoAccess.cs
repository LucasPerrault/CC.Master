namespace Instances.Domain.Demos.Filtering
{

    public abstract class DemoAccess
    {
        protected internal DemoAccess()
        { }

        public static DemoAccess All => new AllDemosAccess();

        public static DemoAccess ForDistributor
            (string distributorId) => new DistributorDemosAccess(distributorId);

        public static DemoAccess None => new NoDemosAccess();
    }

    public class AllDemosAccess : DemoAccess
    { }

    public class DistributorDemosAccess : DemoAccess
    {

        public string DistributorCode { get; }
        public DistributorDemosAccess(string distributorCode)
        {
            DistributorCode = distributorCode;
        }
    }

    public class NoDemosAccess : DemoAccess
    { }
}
