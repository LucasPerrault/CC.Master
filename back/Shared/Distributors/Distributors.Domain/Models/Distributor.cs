namespace Distributors.Domain.Models
{
    public static class DistributorIds
    {
        public const string PeopleSphere = "0015700001sILhMAAW";
        public const string Lucca = "0015700001sJ2d5AAC";
    }

    public class Distributor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DepartmentId { get; set; }
    }
}
