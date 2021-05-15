using Authentication.Domain;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using System.Collections.Generic;
using System.Security.Claims;
using Users.Domain;

namespace Tools.Specflow
{
    public class TestPrincipal
    {
        public ClaimsPrincipal Principal { get; }
        public Dictionary<Operation, Scope> OperationsWithScope { get; }

        public TestPrincipal(string departmentCode)
        : this(departmentCode, new Dictionary<Operation, Scope>())
        { }

        public TestPrincipal(string departmentCode, Dictionary<Operation , Scope> operationsWithScope)
        {
            Principal = new CloudControlUserClaimsPrincipal(new Principal
            {
                UserId = 1,
                User = new User
                {
                    DepartmentCode = departmentCode,
                    FirstName = "Jean",
                    LastName = "Bombeur",
                }
            });

            OperationsWithScope = operationsWithScope;
        }

        public TestPrincipal Add(Operation operation, Scope scope)
        {
            OperationsWithScope[operation] = scope;
            return this;
        }
    }
}
