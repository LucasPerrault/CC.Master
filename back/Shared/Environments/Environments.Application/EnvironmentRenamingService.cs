using Authentication.Domain;
using Environments.Domain;
using Environments.Domain.ExtensionInterface;
using Environments.Domain.Storage;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Environments.Application
{
    public class EnvironmentRenamingService : IEnvironmentRenamingService
    {
        private readonly IEnvironmentsStore _environmentStore;
        private readonly IEnvironmentsRenamingStore _environmentRenamingStore;
        private readonly IEnumerable<IEnvironmentRenamingExtension> _environmentRenamingExtensions;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public EnvironmentRenamingService(
            IEnvironmentsStore environmentStore, IEnvironmentsRenamingStore environmentRenamingStore,
            IEnumerable<IEnvironmentRenamingExtension> environmentRenamingExtensions, ClaimsPrincipal claimsPrincipal)
        {
            _environmentStore = environmentStore;
            _environmentRenamingStore = environmentRenamingStore;
            _environmentRenamingExtensions = environmentRenamingExtensions;
            _claimsPrincipal = claimsPrincipal;
        }

        public async Task RenameAsync(int environmentId, string newName)
        {
            var environement = (await _environmentStore.GetAsync(new EnvironmentFilter
            {
                IsActive = Tools.CompareBoolean.TrueOnly,
                Ids = new HashSet<int> { environmentId }
            })).SingleOrDefault();
            if (environement == null)
            {
                throw new DomainException(DomainExceptionCode.BadRequest, $"Environment {environmentId} is not found");
            }

            await _environmentStore.UpdateSubDomainAsync(environement, newName);

            var environmentRenaming = new EnvironmentRenaming
            {
                Environment = environement,
                OldName = environement.Subdomain,
                NewName = newName,
                RenamedOn = DateTime.UtcNow
            };
            _ = _claimsPrincipal switch
            {
                CloudControlUserClaimsPrincipal u => environmentRenaming.UserId = u.UserId,
                CloudControlApiKeyClaimsPrincipal ak => environmentRenaming.ApiKeyId = ak.ApiKey.Id,
                _ => throw new NotImplementedException($"Claims principal type not implemented : {_claimsPrincipal.GetType()}")
            };
            await _environmentRenamingStore.CreateAsync(environmentRenaming);

            foreach (var environmentRenamingExtension in _environmentRenamingExtensions)
            {
                await environmentRenamingExtension.RenameAsync(environement, newName);
            }
        }
    }
}
