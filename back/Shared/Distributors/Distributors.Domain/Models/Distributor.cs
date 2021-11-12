namespace Distributors.Domain.Models
{
    public static class DistributorIds
    {
        public const int PeopleSphere = 36;
        public const int Lucca = 37;
    }

    public class Distributor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DepartmentId { get; set; }
        public bool IsAllowingCommercialCommunication { get; set; }
        public bool IsActive { get; set; }
    }
}
