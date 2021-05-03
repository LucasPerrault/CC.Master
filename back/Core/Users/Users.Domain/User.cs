using Lucca.Core.Rights.Abstractions.Principals;

namespace Users.Domain
{
    public class User : IUser
    {
        private const string LuccaDepartmentCode = "LUC";

        public int Id { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsLuccaUser => DepartmentCode == LuccaDepartmentCode;
        public int? ManagerId { get; set; }
        public int DepartmentId { get; set; }
        public int LegalEntityId { get; set; }
    }
}
