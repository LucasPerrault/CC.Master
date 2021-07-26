using FluentAssertions;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rights.Infra.Tests.Extensions
{
    public class PermissionsExtensionsTests
    {

        [Theory]
        [ClassData(typeof(PermissionGenerator))]
        public void PermissionSerializationShouldWork(Permission permission)
        {
            permission.ToBytes().ToPermission().Should().BeEquivalentTo(permission);
        }

        [Fact]
        public void PermissionsSerializationShouldWork()
        {
            PermissionGenerator.Permissions
                .ToBytes()
                .ToPermissions()
                .Should().BeEquivalentTo(PermissionGenerator.Permissions);
        }

        [Theory]
        [ClassData(typeof(ApiKeyPermissionGenerator))]
        public void ApiKeyPermissionSerializationShouldWork(ApiKeyPermission permission)
        {
            permission.ToBytes().ToApiKeyPermission().Should().BeEquivalentTo(permission);
        }

        [Fact]
        public void ApiKeyPermissionsSerializationShouldWork()
        {
            ApiKeyPermissionGenerator.ApiKeyPermissions
                .ToBytes()
                .ToApiKeyPermissions()
                .Should().BeEquivalentTo(ApiKeyPermissionGenerator.ApiKeyPermissions);
        }
    }

    public class PermissionGenerator : IEnumerable<object[]>
    {
        public static readonly List<IUserPermission> Permissions = new List<IUserPermission>
        {
            new Permission
            {
                Scope = Scope.AllDepartments,
                OperationId = 3,
                ExternalEntityId = 25,
            },
            new Permission
            {
                Scope = Scope.DepartmentOnly,
                OperationId = 3,
                ExternalEntityId = 25,
            },
            new Permission
            {
                Scope = Scope.DepartementLevel3,
                OperationId = 3,
                ExternalEntityId = 25,
            },
            new Permission
            {
                Scope = Scope.AllDepartments,
                OperationId = 3,
                ExternalEntityId = 25,
                LegalEntityId = 1,
            },
            new Permission
            {
                Scope = Scope.AllDepartments,
                OperationId = 3,
                ExternalEntityId = 25,
                SpecificUserId = 1,
            },
            new Permission
            {
                Scope = Scope.AllDepartments,
                OperationId = 3,
                ExternalEntityId = 25,
                SpecificDepartmentId = 1,
            },
            new Permission
            {
                Scope = Scope.AllDepartments,
                OperationId = 3,
                ExternalEntityId = 25,
                HasContextualLegalEntityAssociation = true,
            }
        };

        public IEnumerator<object[]> GetEnumerator() => Permissions.Select(p => new object[] { p }).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ApiKeyPermissionGenerator : IEnumerable<object[]>
    {
        public static readonly List<IApiKeyPermission> ApiKeyPermissions = new List<IApiKeyPermission>
        {
            new ApiKeyPermission
            {
                OperationId = 3,
                ExternalEntityId = 25,
            },
            new ApiKeyPermission
            {
                OperationId = 3,
                ExternalEntityId = 25,
                LegalEntityId = 1,
            }
        };

        public IEnumerator<object[]> GetEnumerator() => ApiKeyPermissions.Select(p => new object[] { p }).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
