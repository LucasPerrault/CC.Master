using CloudControl.Shared.Domain.Models;

namespace CloudControl.Shared.Infra.Authentication.DTOs
{
    internal class LuccaUser
    {
        public static readonly string ApiFields = $"{nameof(Id)},{nameof(FirstName)},{nameof(LastName)},{nameof(Mail)},{nameof(Login)},{nameof(Department)}[{LuccaDepartment.ApiFields}],{nameof(ManagerId)},{nameof(DepartmentId)},{nameof(LegalEntityId)}";

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public LuccaDepartment Department { get; set; }
        public int? ManagerId { get; set; }
        public int DepartmentId { get; set; }
        public int LegalEntityId { get; set; }
    }

    internal class LuccaDepartment
    {
        public static readonly string ApiFields = $"{nameof(Code)}";

        public string Code { get; set; }
    }

    internal static class LuccaUserExtensions
    {
        public static User ToUser(this LuccaUser user)
        {
            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mail = user.Mail,
                Login = user.Login,
                DepartmentCode = user.Department.Code,
                ManagerId = user.ManagerId,
                DepartmentId = user.DepartmentId,
                LegalEntityId = user.LegalEntityId,
            };
        }
    }
}
