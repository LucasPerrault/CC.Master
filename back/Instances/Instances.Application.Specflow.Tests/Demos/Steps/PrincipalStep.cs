using Distributors.Domain.Models;
using Instances.Application.Specflow.Tests.Demos.Models;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Collections.Generic;
using System.Linq;
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

            var distributorId = _demosContext.DbContext.Set<Distributor>().Single(d => d.Code == departmentCode).Id;
            _demosContext.TestPrincipal = new TestPrincipal(distributorId, operationDict);
        }
    }
}
