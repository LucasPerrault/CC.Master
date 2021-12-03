using Authentication.Domain;
using FluentAssertions;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Lucca.Core.Rights.Abstractions.Stores;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Infra.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Users.Domain;
using Xunit;

namespace Rights.Infra.Tests.Services
{
    public class RightsServiceTests
    {

        [Fact]
        public async Task GetScopedPermissionsAsync_ShouldReturnScopedPermissions()
        {
            var permissionStoreMock = new Mock<IPermissionsStore>();
            permissionStoreMock
                .Setup(ps => ps.GetUserPermissionsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ISet<int>>()))
                .ReturnsAsync((int principalId, int appInstanceId, ISet<int> operations) => operations.SelectMany(o => new List<IUserPermission> {
                    new PurposePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly,
                        ExternalEntityId = 2
                    },
                    new PurposePermission {
                        OperationId = o,
                        Scope = Scope.AllDepartments,
                        ExternalEntityId = 2
                    },
                    new PurposePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly,
                        ExternalEntityId = 2
                    },
                }).ToList());

            var rightsService = new RightsService(
                new RightsHelper((new Mock<IServiceProvider>()).Object),
                new CloudControlUserClaimsPrincipal(new Principal()
                {
                    Token = Guid.NewGuid(),
                    UserId = 1,
                    User = new User()
                    {
                        Id = 1,
                        DepartmentId = 1,
                        EstablishmentId = 1,
                        ManagerId = 0,
                    }
                }),
                permissionStoreMock.Object);

            var scopedPermissions = await rightsService.GetScopedPermissionsAsync(Operation.Demo);

            scopedPermissions.Should().BeEquivalentTo<ScopedPermission>
            (
                new List<ScopedPermission>
                {
                    new ScopedPermission
                    {
                        Operation = Operation.Demo,
                        Scope = AccessRightScope.AllDistributors,
                        EnvironmentPurposes = new HashSet<int> { 2 }
                    },
                    new ScopedPermission
                    {
                        Operation = Operation.Demo,
                        Scope = AccessRightScope.OwnDistributorOnly,
                        EnvironmentPurposes = new HashSet<int> { 2 }
                    }
                }
            );
        }

        [Fact]
        public async Task GetUserOperationsHighestScopeAsyncShouldAlwaysReturnScopeAllDepartmentsAsHighestScopeIfPresentAsync()
        {

            var permissionStoreMock = new Mock<IPermissionsStore>();
            permissionStoreMock
                .Setup(ps => ps.GetUserPermissionsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ISet<int>>()))
                .ReturnsAsync((int principalId, int appInstanceId, ISet<int> operations) => operations.SelectMany(o => new List<IUserPermission> {
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly
                    },
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.AllDepartments
                    },
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly
                    },
                }).ToList());

            var rightsService = new RightsService(
                new RightsHelper((new Mock<IServiceProvider>()).Object),
                new CloudControlUserClaimsPrincipal(new Principal()
                {
                    Token = Guid.NewGuid(),
                    UserId = 1,
                    User = new User()
                    {
                        Id = 1,
                        DepartmentId = 1,
                        EstablishmentId = 1,
                        ManagerId = 0,
                    }
                }),
                permissionStoreMock.Object);

            Assert.All(await rightsService.GetUserOperationsHighestScopeAsync(Operation.Demo, Operation.ReadCMRR), kvp => Assert.Equal(AccessRightScope.AllDistributors, kvp.Value));
        }

        [Fact]
        public async Task GetUserOperationHighestScopeAsyncShouldAlwaysReturnScopeAllDepartmentsAsHighestScopeIfPresentAsync()
        {

            var permissionStoreMock = new Mock<IPermissionsStore>();
            permissionStoreMock
                .Setup(ps => ps.GetUserPermissionsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ISet<int>>()))
                .ReturnsAsync((int principalId, int appInstanceId, ISet<int> operations) => operations.SelectMany(o => new List<IUserPermission> {
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly
                    },
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.AllDepartments
                    },
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly
                    },
                }).ToList());

            var rightsService = new RightsService(
                new RightsHelper((new Mock<IServiceProvider>()).Object),
                new CloudControlUserClaimsPrincipal(new Principal()
                {
                    Token = Guid.NewGuid(),
                    UserId = 1,
                    User = new User()
                    {
                        Id = 1,
                        DepartmentId = 1,
                        EstablishmentId = 1,
                        ManagerId = 0,
                    }
                }),
                permissionStoreMock.Object);

            Assert.Equal(AccessRightScope.AllDistributors, await rightsService.GetUserOperationHighestScopeAsync(Operation.Demo));
        }

        [Fact]
        public async Task GetUserOperationsHighestScopeAsyncShouldThrowAnExceptionWhenAPermissionHasAnUnhandledScopeAsync()
        {

            var permissionStoreMock = new Mock<IPermissionsStore>();
            permissionStoreMock
                .Setup(ps => ps.GetUserPermissionsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ISet<int>>()))
                .ReturnsAsync((int principalId, int appInstanceId, ISet<int> operations) => operations.SelectMany(o => new List<IUserPermission> {
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.User
                    },
                }).ToList());

            var rightsService = new RightsService(
                new RightsHelper((new Mock<IServiceProvider>()).Object),
                new CloudControlUserClaimsPrincipal(new Principal()
                {
                    Token = Guid.NewGuid(),
                    UserId = 1,
                    User = new User()
                    {
                        Id = 1,
                        DepartmentId = 1,
                        EstablishmentId = 1,
                        ManagerId = 0,
                    }
                }),
                permissionStoreMock.Object);
            await Assert.ThrowsAsync<InvalidEnumArgumentException>(async () => await rightsService.GetUserOperationsHighestScopeAsync(Operation.Demo, Operation.ReadCMRR));
        }

        [Fact]
        public async Task GetUserOperationHighestScopeAsyncShouldThrowAnExceptionWhenAPermissionHasAnUnhandledScopeAsync()
        {

            var permissionStoreMock = new Mock<IPermissionsStore>();
            permissionStoreMock
                .Setup(ps => ps.GetUserPermissionsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ISet<int>>()))
                .ReturnsAsync((int principalId, int appInstanceId, ISet<int> operations) => operations.SelectMany(o => new List<IUserPermission> {
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.User
                    },
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.AllDepartments
                    },
                    new SimplePermission {
                        OperationId = o,
                        Scope = Scope.DepartmentOnly
                    },
                }).ToList());

            var rightsService = new RightsService(
                new RightsHelper((new Mock<IServiceProvider>()).Object),
                new CloudControlUserClaimsPrincipal(new Principal()
                {
                    Token = Guid.NewGuid(),
                    UserId = 1,
                    User = new User()
                    {
                        Id = 1,
                        DepartmentId = 1,
                        EstablishmentId = 1,
                        ManagerId = 0,
                    }
                }),
                permissionStoreMock.Object);

            await Assert.ThrowsAsync<InvalidEnumArgumentException>(async () => await rightsService.GetUserOperationHighestScopeAsync(Operation.Demo));
        }


        private class SimplePermission : IUserPermission
        {
            public int OperationId { get; set; }

            public int? EstablishmentId => throw new NotImplementedException();

            public int ExternalEntityId => throw new NotImplementedException();

            public Scope Scope { get; set; }

            public int? SpecificDepartmentId => throw new NotImplementedException();

            public int? SpecificUserId => throw new NotImplementedException();

            public bool HasContextualEstablishmentAssociation => throw new NotImplementedException();
        }


        private class PurposePermission : IUserPermission
        {
            public int OperationId { get; set; }

            public int? EstablishmentId => throw new NotImplementedException();

            public int ExternalEntityId { get; set; }

            public Scope Scope { get; set; }

            public int? SpecificDepartmentId => throw new NotImplementedException();

            public int? SpecificUserId => throw new NotImplementedException();

            public bool HasContextualEstablishmentAssociation => throw new NotImplementedException();
        }
    }
}
