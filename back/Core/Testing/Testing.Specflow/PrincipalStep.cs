using Authentication.Domain;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using Testing.Infra;

namespace Testing.Specflow
{
    [Binding]
    public sealed class PrincipalStep
    {
        private readonly SpecflowTestContext _context;

        public PrincipalStep(SpecflowTestContext context)
        {
            _context = context;
        }

        [Given("a user with department code '(.*)'")]
        public void GivenAUserWithDepartmentCode(string departmentCode)
        {
            var distributorId = _context.Distributors.Single(d => d.Code == departmentCode).Id;
            _context.Principal = TestPrincipal.NewUser(distributorId);
        }

        [Given("user has operation '(.*)' with scope '(.*)'")]
        public void GivenAUserWithOperationAndScope(Operation op, AccessRightScope scope)
        {
            _context.TestPermissionsStore.AddUserPermission(( (CloudControlUserClaimsPrincipal)_context.Principal ).User.Id, op, GetScope(scope));
        }

        private Scope GetScope(AccessRightScope scope)
        {
            return scope switch
            {
                AccessRightScope.AllDistributors => Scope.AllDepartments,
                AccessRightScope.OwnDistributorOnly => Scope.DepartmentOnly,
                _ => throw new NotImplementedException()
            };
        }
    }
}
