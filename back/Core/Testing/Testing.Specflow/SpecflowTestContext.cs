using Distributors.Domain.Models;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Principals;
using Lucca.Core.Rights.Abstractions.Stores;
using Lucca.Core.Rights.DepartmentsHelper;
using Lucca.Core.Rights.ExpressionsHelper;
using Lucca.Core.Rights.RightsHelper;
using Moq;
using Rights.Domain.Abstractions;
using Rights.Infra.Services;
using Rights.Infra.Stores;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Testing.Specflow
{
    public class SpecflowTestContext
    {
        public TestPermissionsStore TestPermissionsStore { get; } = new TestPermissionsStore();
        public ClaimsPrincipal Principal { get; set; }
        public List<Distributor> Distributors { get; set; } = new List<Distributor>();
        public Exception ThrownException { get; set; }

        public IRightsService GetRightsService()
        {
            var permissionStore = new PermissionsStore(TestPermissionsStore);
            var departmentsTreeStoreMock = new Mock<IDepartmentsTreeStore>();
            var departmentsScopeHelper = new DepartmentScopeHelper(departmentsTreeStoreMock.Object);
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(It.Is<Type>(t => t == typeof(IRightsHelper<IUser>)))).Returns(new UserRightsHelper(permissionStore, departmentsScopeHelper, new RightExpressionsHelper(departmentsScopeHelper, new ActorsStore())));
            return new RightsService(new RightsHelper(serviceProviderMock.Object), Principal, permissionStore);
        }
    }
}
