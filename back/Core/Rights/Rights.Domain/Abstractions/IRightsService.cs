using System.Threading.Tasks;

namespace Rights.Domain.Abstractions
{
    public interface IRightsService
    {
        Task<bool> HasOperationAsync(Operation operation);
        Task ThrowIfAnyOperationIsMissingAsync(Operation operation);
    }
}