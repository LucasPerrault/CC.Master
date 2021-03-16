using Authentication.Domain;
using Instances.Application.Specflow.Tests.Demos.Models;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

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
    }
}
