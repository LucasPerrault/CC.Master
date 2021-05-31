using Authentication.Domain;
using Environments.Domain.Storage;
using Lucca.Core.Rights.Abstractions;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain;
using Xunit;

namespace Environments.Domain.Tests
{
    public class EnvironmentRightsFilterTests
    {

        private readonly Mock<IRightsService> _rightsServiceMock = new Mock<IRightsService>();

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task ShouldReturnProperAccessRight(List<ScopedPermission> input, List<EnvironmentAccessRight> output)
        {
            _rightsServiceMock.Setup(s => s.GetScopedPermissionsAsync(Operation.ReadEnvironments))
                .ReturnsAsync(input);

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
            Assert.Equal(output, accessRights);
        }

        public static IEnumerable<object[]> TestData() {
            yield return new object[]
            {
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
                }, new List<EnvironmentAccessRight>
                {
                    new EnvironmentAccessRight
                    (
                        AccessRight.All,
                        PurposeAccessRight.ForSome(EnvironmentPurpose.Contractual)
                    )
                }
            };
        }
    }
}
