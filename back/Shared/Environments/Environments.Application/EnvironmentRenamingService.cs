using Authentication.Domain.Helpers;
using Environments.Domain;
using Environments.Domain.ExtensionInterface;
using Environments.Domain.Storage;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Slack.Domain;
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
        private readonly ILogger<EnvironmentRenamingService> _logger;
        private readonly string _slackChannel;
        private readonly ISlackClient _slackClient;

        public EnvironmentRenamingService(
            IEnvironmentsStore environmentStore, IEnvironmentsRenamingStore environmentRenamingStore,
            IEnumerable<IEnvironmentRenamingExtension> environmentRenamingExtensions, ClaimsPrincipal claimsPrincipal,
            ISlackClient slackClient, EnvironmentRenamingConfiguration environmentRenamingConfiguration,
            ILogger<EnvironmentRenamingService> logger)
        {
            _environmentStore = environmentStore;
            _environmentRenamingStore = environmentRenamingStore;
            _environmentRenamingExtensions = environmentRenamingExtensions;
            _claimsPrincipal = claimsPrincipal;
            _slackClient = slackClient;
            _slackChannel = environmentRenamingConfiguration.SlackChannel;
            _logger = logger;
        }

        public async Task<EnvironmentRenamingStatusDetail> RenameAsync(int environmentId, string newName)
        {
            var environement = (await _environmentStore.GetAsync(EnvironmentAccessRight.Everything, new EnvironmentFilter
            {
                IsActive = Tools.CompareBoolean.TrueOnly,
                Ids = new HashSet<int> { environmentId }
            })).SingleOrDefault();
            if (environement == null)
            {
                throw new DomainException(DomainExceptionCode.BadRequest, $"Environment {environmentId} is not found");
            }

            var slackRenamingText = $"Renaming of {environement.Subdomain} to {newName} by {_claimsPrincipal?.Identity?.Name ?? "unknown"}";
            var slackMessage = await _slackClient.SendMessageAsync(_slackChannel, $":fidget_spinner: {slackRenamingText}");
            try
            {
                await _environmentStore.UpdateSubDomainAsync(environement, newName);
            }
            catch
            {
                await slackMessage.EditMessageAsync(SlackMessageType.ERROR, slackRenamingText);
                throw;
            }

            var exceptionMessages = new List<string>();
            foreach (var environmentRenamingExtension in _environmentRenamingExtensions)
            {
                try
                {
                    _logger.LogDebug("Execution of renamingExtension {extensionName}", environmentRenamingExtension.ExtensionName);
                    await environmentRenamingExtension.RenameAsync(environement, newName);
                    await slackMessage.SendThreadMessageAsync(SlackMessageType.OK, $"{environmentRenamingExtension.ExtensionName}: OK");
                }
                catch (Exception ex)
                {
                    await slackMessage.SendThreadMessageAsync(SlackMessageType.ERROR, $"{environmentRenamingExtension.ExtensionName}: Error");
                    _logger.LogError(ex, "Error during execution of renaming extension {extensionName}", environmentRenamingExtension.ExtensionName);
                    exceptionMessages.Add(ex.Message);
                }
            }

            var status = new EnvironmentRenamingStatusDetail(exceptionMessages);
            var environmentRenaming = new EnvironmentRenaming
            {
                Environment = environement,
                OldName = environement.Subdomain,
                NewName = newName,
                RenamedOn = DateTime.UtcNow,
                UserId = _claimsPrincipal.GetAuthorIdOnlyWhenUser(),
                ApiKeyStorageId = _claimsPrincipal.GetApiKeyStorableId(),
                Status = status.Status,
                ErrorMessage = string.Join(" -- ", status.ErrorMessages)
            };
            await _environmentRenamingStore.CreateAsync(environmentRenaming);

            await slackMessage.EditMessageAsync(
                status.Status == EnvironmentRenamingStatus.SUCCESS ? SlackMessageType.OK : SlackMessageType.WARNING,
                slackRenamingText
            );

            return status;
        }
    }
}
