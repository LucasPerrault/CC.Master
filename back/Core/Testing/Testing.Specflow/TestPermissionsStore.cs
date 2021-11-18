using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Domain;
using Rights.Infra.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testing.Specflow
{
    public class TestPermissionsStore : ICloudControlPermissionsStore
    {
        private List<TestStoredPermission> UserPermissions { get; set; } = new List<TestStoredPermission>();
        private List<TestStoredPermission> ApiKeyPermissions { get; set; } = new List<TestStoredPermission>();
        public Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations)
        {
            return Task.FromResult(ApiKeyPermissions
                .Where(p => p.Id == apiKeyId)
                .Where(p => operations.Contains(p.OperationId))
                .Select(p => (IApiKeyPermission)p.TestPermission)
                .ToList()
            );
        }

        public Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId)
        {
            return Task.FromResult(ApiKeyPermissions
                .Where(p => p.Id == apiKeyId)
                .Select(p => (IApiKeyPermission)p.TestPermission)
                .ToList()
            );
        }

        public Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId)
        {
            return Task.FromResult(UserPermissions
                .Where(p => p.Id == principalId)
                .Select(p => (IUserPermission)p.TestPermission)
                .ToList()
            );
        }

        public Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, ISet<int> operations)
        {
            return Task.FromResult(UserPermissions
                .Where(p => p.Id == principalId)
                .Where(p => operations.Contains(p.OperationId))
                .Select(p => (IUserPermission)p.TestPermission)
                .ToList()
            );
        }

        public void AddUserPermission(int userId, Operation operation, Scope scope)
        {
            UserPermissions.Add(new TestStoredPermission
            {
                Id = userId,
                OperationId = (int)operation,
                TestPermission = new TestPermission
                {
                    Scope = scope,
                    OperationId = (int)operation,
                }
            });
        }


        public void AddApiKeyPermission(int apiKeyId, Operation operation, Scope scope)
        {
            ApiKeyPermissions.Add(new TestStoredPermission
            {
                Id = apiKeyId,
                OperationId = (int)operation,
                TestPermission = new TestPermission
                {
                    Scope = scope,
                    OperationId = (int)operation,
                }
            });
        }

        private class TestStoredPermission
        {
            public int Id { get; set; }
            public int AppInstanceId { get; set; }
            public int OperationId { get; set; }
            public TestPermission TestPermission { get; set; }
        }

        private class TestPermission : IUserPermission, IApiKeyPermission, IWebServicePermission
        {
            public int OperationId { get; init; }
            public Scope Scope { get; init; }

            public int ExternalEntityId => throw new NotImplementedException();
            public int? LegalEntityId => throw new NotImplementedException();
            public int? SpecificDepartmentId => throw new NotImplementedException();
            public int? SpecificUserId => throw new NotImplementedException();
            public bool HasContextualLegalEntityAssociation => throw new NotImplementedException();
        }
    }
}
