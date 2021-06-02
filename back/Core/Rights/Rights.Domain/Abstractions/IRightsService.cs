using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rights.Domain.Abstractions
{
    public interface IRightsService
    {
        Task<bool> HasOperationAsync(Operation operation);
        Task ThrowIfAnyOperationIsMissingAsync(params Operation[] operations);
        Task ThrowIfAllOperationsAreMissingAsync(params Operation[] operations);
        Task<AccessRightScope> GetUserOperationHighestScopeAsync(Operation operation);
        Task<Dictionary<Operation, AccessRightScope>> GetUserOperationsHighestScopeAsync(params Operation[] operations);
        Task<ISet<int>> GetEnvironmentPurposesAsync(params Operation[] operations);
        Task<List<ScopedPermission>> GetScopedPermissionsAsync(Operation operation);
    }
}
