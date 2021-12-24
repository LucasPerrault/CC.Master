using Environments.Domain;
using System.Threading.Tasks;

namespace Environments.Application
{
    public interface IEnvironmentRenamingService
    {
        Task<EnvironmentRenamingStatusDetail> RenameAsync(int environmentId, string newName);
    }
}
