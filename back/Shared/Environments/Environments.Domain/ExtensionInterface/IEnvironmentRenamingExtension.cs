using System.Threading.Tasks;

namespace Environments.Domain.ExtensionInterface
{

    public interface IEnvironmentRenamingExtension
    {
        string ExtensionName { get; }
        bool ShouldExecute(IEnvironmentRenamingExtensionParameters parameters);
        Task RenameAsync(Environment environment, string newName);
    }
}
