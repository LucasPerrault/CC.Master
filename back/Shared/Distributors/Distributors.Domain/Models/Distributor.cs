namespace Distributors.Domain.Models
{
    public static class DistributorIds
    {
        public const string PeopleSphere = "0015700001sILhMAAW";
    }

    public class Distributor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
