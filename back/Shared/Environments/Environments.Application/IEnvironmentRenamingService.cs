using System.Threading.Tasks;

namespace Environments.Application
{
    public interface IEnvironmentRenamingService
    {
        Task RenameAsync(int environmentId, string newName);
    }
}
