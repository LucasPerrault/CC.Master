using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;
using Users.Domain;

namespace Testing.Infra
{
    public class TestPrincipal
    {
        public ClaimsPrincipal Principal { get; }
        public Dictionary<Operation, AccessRightScope> OperationsWithScope { get; }

        public TestPrincipal(string departmentCode)
        : this(departmentCode, new Dictionary<Operation, AccessRightScope>())
        { }

        public TestPrincipal(string departmentCode, Dictionary<Operation , AccessRightScope> operationsWithScope)
        {
            Principal = new CloudControlUserClaimsPrincipal(new Principal
            {
                UserId = 1,
                User = new User
                {
                    DistributorCode = departmentCode,
                    FirstName = "Jean",
                    LastName = "Bombeur",
                }
            });

            OperationsWithScope = operationsWithScope;
        }

        public TestPrincipal Add(Operation operation, AccessRightScope scope)
        {
            OperationsWithScope[operation] = scope;
            return this;
        }
    }
}
