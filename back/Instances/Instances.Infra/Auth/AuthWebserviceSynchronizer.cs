using Authentication.Infra.Services;
using Lucca.Logs.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Instances.Infra.Auth
{
    public interface IAuthWebserviceSynchronizer
    {
        Task SynchronizeAsync(int instanceId);
    }

    public class AuthWebserviceSynchronizer : IAuthWebserviceSynchronizer
    {
        private readonly AuthWebserviceRemoteService _authWebservice;
        private readonly LuccaLogger _logger;

        public AuthWebserviceSynchronizer(AuthWebserviceRemoteService authWebservice, LuccaLogger logger)
        {
            _authWebservice = authWebservice;
            _logger = logger;
        }

        public async Task SynchronizeAsync(int instanceId)
        {
            try
            {
                await SynchronizeInstancesAsync();
                await SynchronizeUsersAsync(instanceId);
            }
            catch { }
        }

        private async Task SynchronizeInstancesAsync()
        {
            try
            {
                await _authWebservice.PostAsync("instances/syncs");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Error while synchronizing authentication WS instances");
                throw;
            }
        }

        private async Task SynchronizeUsersAsync(int instanceId)
        {
            try
            {
                await _authWebservice.PostAsync($"instances/{instanceId}/syncs");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"Error while synchronizing authentication WS instance { instanceId } users");
                throw;
            }
        }
    }
}
