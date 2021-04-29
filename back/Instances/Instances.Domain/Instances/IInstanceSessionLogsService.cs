using System;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface IInstanceSessionLogsService
    {
        Task<DateTime> GetLatestAsync(Uri href);
    }
}
