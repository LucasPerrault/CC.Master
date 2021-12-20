using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsRenamingStore
    {
        Task<EnvironmentRenaming> CreateAsync(EnvironmentRenaming environmentRenaming);
    }
}
