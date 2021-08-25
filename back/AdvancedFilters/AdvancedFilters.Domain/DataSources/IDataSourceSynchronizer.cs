using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceSynchronizer
    {
        Task SyncAsync();
    }
}
