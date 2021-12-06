using Authentication.Domain;
using System.Security.Claims;
using Users.Domain;

namespace Testing.Infra
{
    public class TestPrincipal
    {
        private const int DefaultTestDepartmentId = 1;

        public static ClaimsPrincipal NewUser() => NewUser(DefaultTestDepartmentId);

        public static ClaimsPrincipal NewUser(int departmentId) => new CloudControlUserClaimsPrincipal(new Principal
        {
            UserId = 1,
            User = new User
            {
                DistributorId = departmentId,
                FirstName = "Jean",
                LastName = "Bombeur",
            }
        });
    }
}
