using System.Threading.Tasks;

namespace Users.Domain
{
    public interface IUsersSyncService
    {
        Task SyncAsync();
    }
}
