using Instances.Application.Instances;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly IGithubBranchesRepository _githubBranchesRepository;
        private readonly IGithubPullRequestsRepository _githubPullRequestsRepository;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;
        private readonly IGithubReposStore _githubReposStore;

        public PullRequestWebhookService(
            IGithubBranchesRepository githubBranchesRepository,
            IGithubPullRequestsRepository githubPullRequestRepository, IPreviewConfigurationsRepository previewConfigurationRepository,
            IGithubReposStore githubReposStore
        )
        {
            _githubBranchesRepository = githubBranchesRepository;
            _githubPullRequestsRepository = githubPullRequestRepository;
            _previewConfigurationsRepository = previewConfigurationRepository;
            _githubReposStore = githubReposStore;
        }

        protected override async Task HandleEventAsync(PullRequestWebhookPayload pullRequestEventPayload)
        {
            // Type d'événement de PR non pris en charge
            if (!SupportedActions.Contains(pullRequestEventPayload.Action))
            {
                return;
            }

            var repo = await _githubReposStore.GetByUriAsync(pullRequestEventPayload.Repository.HtmlUrl);
            if (repo is null)
            {
                return;
            }

            switch (pullRequestEventPayload.Action)
            {
                case ActionOpened:
                    await HandleOpenedActionAsync(pullRequestEventPayload, repo);
                    break;
                case ActionReopened:
                    await HandleReopenedActionAsync(pullRequestEventPayload, repo);
                    break;
                case ActionClosed:
                    await HandleClosedActionAsync(pullRequestEventPayload, repo);
                    break;
                case ActionEdited:
                    await HandleEditedActionAsync(pullRequestEventPayload, repo);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleOpenedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, GithubRepo repo)
        {
            var originBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(repo.Id, pullRequestEventPayload.PullRequest.Head.Ref);
            if (originBranch == null)
            {
                // Branche inconnue, on ne fait rien
                return;
            }
            var pullRequest = new GithubPullRequest
            {
                Number = pullRequestEventPayload.Number,
                Title = pullRequestEventPayload.PullRequest.Title,
                IsOpened = true,
                OpenedAt = DateTime.Now,
                OriginBranchId = originBranch.Id
            };

            pullRequest = await _githubPullRequestsRepository.CreateAsync(pullRequest);
            await _previewConfigurationsRepository.CreateByPullRequestAsync(pullRequest, originBranch);
        }

        private async Task HandleReopenedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, GithubRepo repo)
        {
            var originBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(repo.Id, pullRequestEventPayload.PullRequest.Head.Ref);
            if (originBranch == null)
            {
                // Branche inconnue, on ne fait rien
                return;
            }
            var pullRequest = await _githubPullRequestsRepository.GetByNumberAsync(repo.Id, pullRequestEventPayload.Number);
            if (pullRequest == null)
            {
                // PR inconnue
                return;
            }

            pullRequest.IsOpened = true;
            pullRequest = await _githubPullRequestsRepository.UpdateAsync(pullRequest);
            await _previewConfigurationsRepository.CreateByPullRequestAsync(pullRequest, originBranch);
        }

        private async Task HandleClosedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, GithubRepo repo)
        {
            var pullRequest = await _githubPullRequestsRepository.GetByNumberAsync(repo.Id, pullRequestEventPayload.Number);
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

        private async Task HandleEditedActionAsync(PullRequestWebhookPayload pullRequestEventPayload, GithubRepo repo)
        {
            var originBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(repo.Id, pullRequestEventPayload.PullRequest.Head.Ref);
            if (originBranch == null)
            {
                // Branche inconnue, on ne fait rien
                return;
            }
            var pullRequest = await _githubPullRequestsRepository.GetByNumberAsync(repo.Id, pullRequestEventPayload.Number);
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
