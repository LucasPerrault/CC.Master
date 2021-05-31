using Authentication.Domain;
using Environments.Domain.Storage;
using Lucca.Core.Rights.Abstractions;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Domain;
using Xunit;

namespace Environments.Domain.Tests
{
    public class EnvironmentRightsFilterTests
    {

        private readonly Mock<IRightsService> _rightsServiceMock = new Mock<IRightsService>();

        [Fact]
        public async Task ShouldReturnProperAccessRight()
        {
            _rightsServiceMock.Setup(s => s.GetScopedPermissionsAsync(Operation.ReadEnvironments))
                .ReturnsAsync
                (
                    new List<ScopedPermission>
                    {
                        new ScopedPermission
                        {
                            Operation = Operation.ReadEnvironments,
                            Scope = Scope.AllDepartments,
                            EnvironmentPurposes = new HashSet<int>
                            {
                                (int)EnvironmentPurpose.Contractual
                            }
                        }
                    }
                );
            var envRightsFilter = new EnvironmentRightsFilter(_rightsServiceMock.Object);

            var principal = new CloudControlUserClaimsPrincipal(new Principal
            {
                UserId = 1,
                User = new User
                {
                    FirstName = "Bernard",
                    LastName = "Martin"
                }
            });

            var accessRights = await envRightsFilter.GetAccessRightAsync(principal, Operation.ReadEnvironments);

            Assert.Single(accessRights);

            Assert.Equal(
                new EnvironmentAccessRight
                (
                    AccessRight.All,
                    PurposeAccessRight.ForSome(EnvironmentPurpose.Contractual)
                ),
                accessRights.Single());
        }
    }
}
