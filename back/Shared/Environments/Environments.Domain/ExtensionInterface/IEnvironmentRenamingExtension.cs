using System.Threading.Tasks;

namespace Environments.Domain.ExtensionInterface
{
    public interface IEnvironmentRenamingExtension
    {
        Task RenameAsync(Environment environment, string newName);
    }
}
