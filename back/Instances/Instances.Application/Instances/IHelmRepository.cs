using Instances.Application.Instances.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IHelmRepository
    {
        Task CreateHelmAsync(string releaseName, string branchName, string helmChart);
        Task<List<HelmRelease>> GetAllReleasesAsync(HelmRequest helmRequest);
    }
}
