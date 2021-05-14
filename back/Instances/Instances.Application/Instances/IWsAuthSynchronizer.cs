using System.Threading.Tasks;

namespace Instances.Application.Instances
{

    public interface IWsAuthSynchronizer
    {
        Task SafeSynchronizeAsync(int instanceId);
    }
}
