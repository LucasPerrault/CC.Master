using Instances.Domain.Github.Models;
using Instances.Domain.Preview.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Preview
{
    public interface IPreviewConfigurationsStore
    {
        Task CreateAsync(IEnumerable<PreviewConfiguration> previewConfigurations);
        Task DeleteByBranchAsync(GithubBranch branch);
    }
}
