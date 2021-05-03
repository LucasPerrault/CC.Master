using Authentication.Domain;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Lucca.Core.Rights.Abstractions.Stores;
using Lucca.Core.Rights.RightsHelper;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
    public class RightsService : IRightsService
    {
        private readonly ClaimsPrincipalRightsHelper _rightsHelper;
        private readonly ClaimsPrincipal _principal;
        private readonly IPermissionsStore _permissionsStore;

        private readonly static Dictionary<Scope, int> ScopeRankings = new Dictionary<Scope, int>
        {
            { Scope.AllDepartments, 0 },
            { Scope.DepartmentOnly, 1 },
        };

        public RightsService(ClaimsPrincipalRightsHelper rightsHelper, ClaimsPrincipal principal, IPermissionsStore permissionsStore)
        {
            _rightsHelper = rightsHelper ?? throw new ArgumentNullException(nameof(rightsHelper));
            _principal = principal ?? throw new ArgumentNullException(nameof(principal));
            _permissionsStore = permissionsStore ?? throw new ArgumentNullException(nameof(permissionsStore));
        }

        public async Task ThrowIfAnyOperationIsMissingAsync(params Operation[] operations)
        {
            var missingOps = new List<Operation>();
            foreach (var operation in operations)
            {
                if (!await HasOperationAsync(operation))
                {
                    missingOps.Add(operation);
                }
            }

            if (missingOps.Any())
            {
                throw new MissingOperationsException(missingOps.ToArray());
            }
        }

        public async Task ThrowIfAllOperationsAreMissingAsync(params Operation[] operations)
        {
            foreach (var operation in operations)
            {
                if (await HasOperationAsync(operation))
                {
                    return;
                }
            }
            throw new MissingOperationsException(operations);
        }

        public async Task<bool> HasOperationAsync(Operation operation)
        {
            return await _rightsHelper.HasOperationAsync(_principal, RightsHelper.CloudControlAppInstanceId, (int)operation);
        }

        public async Task<Scope> GetUserOperationHighestScopeAsync(Operation operation)
        {
            return (await GetUserOperationsHighestScopeAsync(operation)).Select(kvp => kvp.Value).FirstOrDefault();
        }

        public async Task<Dictionary<Operation, Scope>> GetUserOperationsHighestScopeAsync(params Operation[] operations)
        {
            var operationsSet = operations.Select(o => (int)o).ToHashSet();
            switch(_principal)
            {
                case CloudControlUserClaimsPrincipal userPrincipal:
                    return (await _permissionsStore.GetUserPermissionsAsync(userPrincipal.User.Id, RightsHelper.CloudControlAppInstanceId, operationsSet))
                        .GroupBy(u => (Operation)u.OperationId)
                        .ToDictionary(g => g.Key, GetHighestScope);
                case CloudControlApiKeyClaimsPrincipal apiKey:
                    throw new ApplicationException("ApiKeys don't have scopes");
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            };
        }

        public async Task<ISet<int>> GetEnvironmentPurposesAsync(params Operation[] operations)
        {
            var operationsSet = operations.Select(o => (int)o).ToHashSet();
            return _principal switch
            {
                CloudControlUserClaimsPrincipal userPrincipal =>
                    (await _permissionsStore.GetUserPermissionsAsync(userPrincipal.User.Id, RightsHelper.CloudControlAppInstanceId, operationsSet))
                        .Select(up => up.ExternalEntityId)
                        .ToHashSet(),
                CloudControlApiKeyClaimsPrincipal apiKey =>
                    (await _permissionsStore.GetApiKeyPermissionsAsync(apiKey.ApiKey.Id, RightsHelper.CloudControlAppInstanceId, operationsSet))
                        .Select(akp => akp.ExternalEntityId)
                        .ToHashSet(),
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }

        private Scope GetHighestScope(IGrouping<Operation, IUserPermission> permissions)
        {
            return permissions.Select(p => p.Scope).OrderBy(GetRank).First();
        }

        private static int GetRank(Scope scope)
        {
            if(!ScopeRankings.ContainsKey(scope))
            {
                throw new ApplicationException($"Scope non pris en charge : '{scope}'");
            }
            return ScopeRankings[scope];
        }
    }
}
