using Instances.Application.Specflow.Tests.Demos.Models;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Testing.Infra;

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
        public void GivenAUserWithOperationAndScope(string departmentCode, Operation op, AccessRightScope scope)
        {
            var operationDict = new Dictionary<Operation, AccessRightScope>
            {
                [op] = scope
            };

            _demosContext.TestPrincipal = new TestPrincipal(departmentCode, operationDict);
        }
    }
}
