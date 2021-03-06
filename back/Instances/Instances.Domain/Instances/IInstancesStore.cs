using Instances.Domain.Instances.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstancesStore
    {
        Instance GetActiveInstanceFromEnvironmentId(int environmentId, InstanceType instanceType);
        Task<Instance> CreateForTrainingAsync(int environmentId, bool isAnonymized);
        Task<Instance> CreateForDemoAsync(string password);
        Task DeleteByIdAsync(int instanceId);
        Task DeleteByIdsAsync(IEnumerable<int> instanceIds);
    }
}
