using System.Linq;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        IQueryable<Environment> GetFiltered(EnvironmentAccessRight accessRight);
    }
}
