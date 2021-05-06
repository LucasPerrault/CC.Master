using Instances.Application.Instances;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Instances.Infra.WsAuth
{
    public class WsAuthSynchronizer : IWsAuthSynchronizer
    {
        private readonly WsAuthRemoteService _authWebservice;
        private readonly ILogger<WsAuthSynchronizer> _logger;

        public WsAuthSynchronizer(WsAuthRemoteService authWebservice, ILogger<WsAuthSynchronizer> logger)
        {
            _authWebservice = authWebservice;
            _logger = logger;
        }

        public async Task SafeSynchronizeAsync(int instanceId)
        {
            try
            {
                await SynchronizeAsync(instanceId);
            }
            catch { }
        }

        private async Task SynchronizeAsync(int instanceId)
        {
            await SynchronizeInstancesAsync();
            await SynchronizeUsersAsync(instanceId);
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
