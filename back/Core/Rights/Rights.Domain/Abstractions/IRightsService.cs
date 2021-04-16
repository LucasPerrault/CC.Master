using Lucca.Core.Rights.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rights.Domain.Abstractions
{
    public interface IRightsService
    {
        Task<bool> HasOperationAsync(Operation operation);
        Task ThrowIfAnyOperationIsMissingAsync(params Operation[] operations);
        Task ThrowIfAllOperationsAreMissingAsync(params Operation[] operations);
        Task<Scope> GetUserOperationHighestScopeAsync(Operation operation);
        Task<Dictionary<Operation, Scope>> GetUserOperationsHighestScopeAsync(params Operation[] operations);
        Task<ISet<int>> GetEnvironmentPurposesAsync(params Operation[] operations);
    }
}
