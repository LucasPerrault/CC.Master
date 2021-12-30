using System.Threading.Tasks;

namespace Environments.Domain.ExtensionInterface
{
    public interface IEnvironmentRenamingExtension
    {
        string ExtensionName { get; }
        Task RenameAsync(Environment environment, string newName);
    }
}
