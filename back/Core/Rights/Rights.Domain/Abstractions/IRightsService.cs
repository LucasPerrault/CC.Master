using System.Threading.Tasks;

namespace Rights.Domain.Abstractions
{
    public interface IRightsService
    {
        Task<bool> HasOperationAsync(Operation operation);
        Task ThrowIfAnyOperationIsMissingAsync(params Operation[] operations);
        Task ThrowIfAllOperationsAreMissingAsync(params Operation[] operations);
    }
}