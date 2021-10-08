using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Webhooks.Github
{
    public class PullRequestWebhookService : GithubWebhookBaseService<PullRequestWebhookPayload>
    {
        private const string ActionOpened = "opened";
        private const string ActionReopened = "reopened";
        private const string ActionClosed = "closed";
        private const string ActionEdited = "edited";

        private static readonly ReadOnlyCollection<string> SupportedActions = new List<string>() { ActionOpened, ActionReopened, ActionClosed, ActionEdited }.AsReadOnly();

        private readonly ICodeSourcesRepository _codeSourcesRepository;
        private readonly IGithubBranchesRepository _githubBranchesRepository;
        private readonly IGithubPullRequestsRepository _githubPullRequestsRepository;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;

        public PullRequestWebhookService(
            ICodeSourcesRepository codeSourcesRepository, IGithubBranchesRepository githubBranchesRepository,
            IGithubPullRequestsRepository githubPullRequestRepository, IPreviewConfigurationsRepository previewConfigurationRepository)
        {
            _codeSourcesRepository = codeSourcesRepository;
            _githubBranchesRepository = githubBranchesRepository;
            _githubPullRequestsRepository = githubPullRequestRepository;
            _previewConfigurationsRepository = previewConfigurationRepository;
        }

        protected override async Task HandleEventAsync(PullRequestWebhookPayload pullRequestEventPayload)
        {
            // Type d'événement de PR non pris en charge
            if (!SupportedActions.Contains(pullRequestEventPayload.Action))
            {
                return;
            }

            var codeSources = await _codeSourcesRepository.GetNonDeletedByRepositoryUrlAsync(pullRequestEventPayload.Repository.HtmlUrl);
            // Repo inconnu, on ne fait rien
            if (codeSources.Count == 0)
            {
                return;
            }

            switch (pullRequestEventPayload.Action)
            {
                case ActionOpened:
                    await HandleOpenedActionAsync(pullRequestEventPayload, codeSources);
                    break;
                case ActionReopened:
                    await HandleReopenedActionAsync(pullRequestEventPayload, codeSources);
                    break;
                case ActionClosed:
                    await HandleClosedActionAsync(pullRequestEventPayload, codeSources);
                    break;
                case ActionEdited:
                    await HandleEditedActionAsync(pullRequestEventPayload, codeSources);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleOpenedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, List<CodeSource> codeSources)
        {
            var originBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(codeSources.First(), pullRequestEventPayload.PullRequest.Head.Ref);
            if (originBranch == null)
            {
                // Branche inconnue, on ne fait rien
                return;
            }
            var pullRequest = new GithubPullRequest
            {
                CodeSources = codeSources,
                Number = pullRequestEventPayload.Number,
                Title = pullRequestEventPayload.PullRequest.Title,
                IsOpened = true,
                OpenedAt = DateTime.Now,
                OriginBranchId = originBranch.Id
            };

            pullRequest = await _githubPullRequestsRepository.CreateAsync(pullRequest);
            await _previewConfigurationsRepository.CreateByPullRequestAsync(pullRequest, originBranch);
        }

        private async Task HandleReopenedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, List<CodeSource> codeSources)
        {
            var originBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(codeSources.First(), pullRequestEventPayload.PullRequest.Head.Ref);
            if (originBranch == null)
            {
                // Branche inconnue, on ne fait rien
                return;
            }
            var pullRequest = await _githubPullRequestsRepository.GetByNumberAsync(codeSources.First(), pullRequestEventPayload.Number);
            if (pullRequest == null)
            {
                // PR inconnue
                return;
            }

            pullRequest.IsOpened = true;
            pullRequest = await _githubPullRequestsRepository.UpdateAsync(pullRequest);
            await _previewConfigurationsRepository.CreateByPullRequestAsync(pullRequest, originBranch);
        }

        private async Task HandleClosedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, List<CodeSource> codeSources)
        {
            var pullRequest = await _githubPullRequestsRepository.GetByNumberAsync(codeSources.First(), pullRequestEventPayload.Number);
            if (pullRequest == null)
            {
                // PR inconnue
                return;
            }
            pullRequest.IsOpened = false;
            if (pullRequestEventPayload.PullRequest.MergedAt.HasValue)
            {
                pullRequest.MergedAt = pullRequestEventPayload.PullRequest.MergedAt.Value.DateTime;
            }
            else
            {
                pullRequest.ClosedAt = pullRequestEventPayload.PullRequest.ClosedAt.Value.DateTime;
            }
            await _githubPullRequestsRepository.UpdateAsync(pullRequest);
        }

        private async Task HandleEditedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, List<CodeSource> codeSources)
        {
            var originBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(codeSources.First(), pullRequestEventPayload.PullRequest.Head.Ref);
            if (originBranch == null)
            {
                // Branche inconnue, on ne fait rien
                return;
            }
            var pullRequest = await _githubPullRequestsRepository.GetByNumberAsync(codeSources.First(), pullRequestEventPayload.Number);
            if (pullRequest == null)
            {
                // PR inconnue
                return;
            }

            pullRequest.Title = pullRequestEventPayload.PullRequest.Title;
            await _githubPullRequestsRepository.UpdateAsync(pullRequest);
        }
    }
}
