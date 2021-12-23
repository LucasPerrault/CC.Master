using Distributors.Domain.Models;
using Lucca.Core.Rights.Abstractions.Principals;

namespace Users.Domain
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public int? ManagerId { get; set; }
        public int DepartmentId { get; set; }
        public int EstablishmentId { get; set; }
        public Distributor Distributor { get; set; }
    }
}
