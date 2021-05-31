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
        private const string DepartmentCode = "DepCode";

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
                    LastName = "Martin",
                    DepartmentCode = DepartmentCode
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

            yield return new object[]
            {
                new List<ScopedPermission>
                {
                    new ScopedPermission
                    {
                        Scope = Scope.DepartmentOnly,
                        EnvironmentPurposes = new HashSet<int>
                        {
                            (int)EnvironmentPurpose.InternalUse
                        }
                    }
                }, new List<EnvironmentAccessRight>
                {
                    new EnvironmentAccessRight
                    (
                        AccessRight.ForDistributor(DepartmentCode),
                        PurposeAccessRight.ForSome(EnvironmentPurpose.InternalUse)
                    )
                }
            };

            yield return new object[]
            {
                new List<ScopedPermission>
                {
                    new ScopedPermission
                    {
                        Scope = Scope.AllDepartments,
                        EnvironmentPurposes = new HashSet<int>
                        {
                            (int)EnvironmentPurpose.Contractual
                        }
                    },
                    new ScopedPermission
                    {
                        Scope = Scope.DepartmentOnly,
                        EnvironmentPurposes = new HashSet<int>
                        {
                            (int)EnvironmentPurpose.Lucca,
                            (int)EnvironmentPurpose.Security,
                            (int)EnvironmentPurpose.Virgin
                        }
                    }
                }, new List<EnvironmentAccessRight>
                {
                    new EnvironmentAccessRight
                    (
                        AccessRight.All,
                        PurposeAccessRight.ForSome(EnvironmentPurpose.Contractual)
                    ),
                    new EnvironmentAccessRight
                    (
                        AccessRight.ForDistributor(DepartmentCode),
                        PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca, EnvironmentPurpose.Security, EnvironmentPurpose.Virgin)
                    )
                }
            };

            yield return new object[] { new List<ScopedPermission>(), new List<EnvironmentAccessRight>() };
        }
    }
}
