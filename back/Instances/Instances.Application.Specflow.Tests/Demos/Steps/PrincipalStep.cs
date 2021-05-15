using Instances.Application.Specflow.Tests.Demos.Models;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using System.Collections;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Tools.Specflow;

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
            var operationDict = new Dictionary<Operation, Scope>
            {
                [op] = scope
            };

            _demosContext.TestPrincipal = new TestPrincipal(departmentCode, operationDict);
        }
    }
}
