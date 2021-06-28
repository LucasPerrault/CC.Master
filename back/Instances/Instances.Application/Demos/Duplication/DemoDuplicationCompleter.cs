﻿using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Instances.Application.Demos.Duplication
{
    public interface IDemoDuplicationCompleter
    {
        Task MarkDuplicationAsCompletedAsync(Guid instanceDuplicationId, bool isSuccessful);
    }

    public class DemoDuplicationCompleter : IDemoDuplicationCompleter
    {
        private readonly IDemosStore _demosStore;
        private readonly IInstancesStore _instancesStore;
        private readonly IDemoDuplicationsStore _duplicationsStore;
        private readonly IInstanceDuplicationsStore _instanceDuplicationsStore;
        private readonly IWsAuthSynchronizer _wsAuthSynchronizer;
        private readonly IDemoUsersPasswordResetService _demoUsersPasswordResetService;
        private readonly IDemoDeletionCalculator _deletionCalculator;
        private readonly ILogger<DemoDuplicationCompleter> _logger;

        public DemoDuplicationCompleter
        (
            IDemoDuplicationsStore duplicationsStore,
            IInstanceDuplicationsStore instanceDuplicationsStore,
            IDemosStore demosStore,
            IInstancesStore instancesStore,
            IWsAuthSynchronizer wsAuthSynchronizer,
            IDemoUsersPasswordResetService demoUsersPasswordResetService,
            IDemoDeletionCalculator deletionCalculator,
            ILogger<DemoDuplicationCompleter> logger
        )
        {
            _duplicationsStore = duplicationsStore;
            _instanceDuplicationsStore = instanceDuplicationsStore;
            _demosStore = demosStore;
            _instancesStore = instancesStore;
            _wsAuthSynchronizer = wsAuthSynchronizer;
            _demoUsersPasswordResetService = demoUsersPasswordResetService;
            _logger = logger;
            _deletionCalculator = deletionCalculator;
        }

        public async Task MarkDuplicationAsCompletedAsync(Guid instanceDuplicationId, bool isSuccessful)
        {
            var duplication = _duplicationsStore.GetByInstanceDuplicationId(instanceDuplicationId);

            if (duplication.HasEnded)
            {
                throw new BadRequestException($"Duplication {instanceDuplicationId} was already marked as complete");
            }

            try
            {
                var instance = await _instancesStore.CreateForDemoAsync(duplication.Password, duplication.InstanceDuplication.TargetCluster);
                await CreateAndStoreDemoAsync(duplication, instance);
                await _instanceDuplicationsStore.MarkAsCompleteAsync(duplication.InstanceDuplication, InstanceDuplicationProgress.FinishedWithSuccess);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Demo creation failed");
                await _instanceDuplicationsStore.MarkAsCompleteAsync(duplication.InstanceDuplication, InstanceDuplicationProgress.FinishedWithFailure);
            }
        }

        private async Task CreateAndStoreDemoAsync(DemoDuplication duplication, Instance instance)
        {
            var demo = BuildDemo(duplication, instance);
            try
            {
                await _demosStore.CreateAsync(demo);
                await _demoUsersPasswordResetService.ResetPasswordAsync(demo, duplication.Password);
                await _wsAuthSynchronizer.SafeSynchronizeAsync(instance.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not create demo, following instance duplication");
                await _instancesStore.DeleteForDemoAsync(instance);
                await _demosStore.DeleteAsync(demo);
                throw;
            }
        }

        private Demo BuildDemo(DemoDuplication duplication, Instance instance)
        {
            var demo = new Demo
            {
                Subdomain = duplication.InstanceDuplication.TargetSubdomain,
                DistributorID = duplication.DistributorId,
                Comment = duplication.Comment,
                CreatedAt = DateTime.Now,
                AuthorId = duplication.AuthorId,
                IsActive = true,
                IsTemplate = false,
                InstanceID = instance.Id
            };

            demo.DeletionScheduledOn = _deletionCalculator.GetDeletionDate(demo, DateTime.Now);
            return demo;
        }
    }
}
