using Lucca.Core.Shared.Domain.Exceptions;
using Medallion.Threading.SqlServer;
using System;
using System.Threading.Tasks;

namespace Lock.Web
{
    public class LockService : ILockService
    {
        private readonly string _databaseConnectionString;

        public LockService(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<IDisposable> TakeLockAsync(string lockName, TimeSpan timeout)
        {
            var distributedLock = new SqlDistributedLock(lockName, _databaseConnectionString);
            var disposableAcquiredLock = await distributedLock.TryAcquireAsync(timeout);
            if (disposableAcquiredLock is null)
            {
                throw new DomainException(DomainExceptionCode.Conflict, "Resource is busy");
            }
            return disposableAcquiredLock;
        }
    }
}
