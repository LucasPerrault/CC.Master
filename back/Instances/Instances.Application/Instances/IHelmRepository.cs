using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IHelmRepository
    {
        Task CreateHelmAsync(string releaseName, string branchName, string helmChart);
    }
}
