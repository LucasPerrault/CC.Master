using System;
using System.Threading.Tasks;

namespace Lock
{
    public interface ILockService
    {
        Task<IDisposable> TakeLockAsync(string lockName, TimeSpan timeout);
    }
}
