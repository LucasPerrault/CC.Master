namespace Distributors.Domain.Models
{
    public class Distributor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsLucca { get; set; }
        public int DepartmentId { get; set; }
        public bool IsAllowingCommercialCommunication { get; set; }
        public bool IsActive { get; set; }
    }
}
