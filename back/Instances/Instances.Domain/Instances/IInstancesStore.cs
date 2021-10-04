using Instances.Domain.Instances.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstancesStore
    {
        Task<Instance> CreateForDemoAsync(string password);
        Task DeleteForDemoAsync(Instance demoInstance);
        Task DeleteForDemoAsync(IEnumerable<Instance> demoInstances);
    }
}
