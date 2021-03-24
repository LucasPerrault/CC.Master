using Instances.Domain.Instances.Models;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstancesStore
    {
        Task<Instance> CreateForDemoAsync(string password, string cluster);
    }
}
