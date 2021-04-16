using Authentication.Domain;
using Instances.Application.Specflow.Tests.Demos.Models;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Users.Domain;
using Xunit;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    [Binding]
    public sealed class PrincipalStep
    {
        private readonly DemosContext _demosContext;

        public PrincipalStep(DemosContext demosContext)
        {
            _demosContext = demosContext;
        }

        [Given("a user with department code '(.*)' and operation '(.*)' and scope '(.*)'")]
        public void GivenAUserWithOperationAndScope(string departmentCode, Operation op, Scope scope)
        {
            _demosContext.OperationsWithScope = new Dictionary<Operation, Scope>
            {
                { op, scope }
            };

            _demosContext.Principal = new CloudControlUserClaimsPrincipal(new Principal()
            {
                UserId = 1,
                User = new User()
                {
                    DepartmentCode = departmentCode,
                    FirstName = "Jean",
                    LastName = "Bombeur",
                }
            });
        }

        [Then(@"user should get error containing '(.*)'")]
        public void ThenUserShouldGetErrorContainingAsync(string errorMessageExtract)
        {
            Assert.Contains(errorMessageExtract, _demosContext.ExceptionResult.Message);
        }

        [Then(@"user should not get error")]
        public void ThenUserShouldNotGetError()
        {
            Assert.Null(_demosContext.ExceptionResult);
        }
    }
}
