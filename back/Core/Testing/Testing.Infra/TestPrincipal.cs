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
        private const int DefaultTestDepartmentId = 1;

        public ClaimsPrincipal Principal { get; }
        public Dictionary<Operation, AccessRightScope> OperationsWithScope { get; }

        public TestPrincipal(int departmentId)
            : this(departmentId, new Dictionary<Operation, AccessRightScope>())
        { }

        public TestPrincipal()
            : this(DefaultTestDepartmentId, new Dictionary<Operation, AccessRightScope>())
        { }

        public TestPrincipal(int departmentId, Dictionary<Operation , AccessRightScope> operationsWithScope)
        {
            Principal = new CloudControlUserClaimsPrincipal(new Principal
            {
                UserId = 1,
                User = new User
                {
                    DistributorId = departmentId,
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
