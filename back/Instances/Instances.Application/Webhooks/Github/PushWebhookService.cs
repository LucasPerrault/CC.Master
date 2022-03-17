using Instances.Application.Instances;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using System;
using System.Threading.Tasks;

namespace Instances.Application.Webhooks.Github
{
    public class PushWebhookService : GithubWebhookBaseService<PushWebhookPayload>
    {
        private readonly IGithubBranchesRepository _githubBranchesRepository;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;
        private readonly IGithubReposStore _githubReposStore;

        public PushWebhookService(
            IGithubBranchesRepository githubBranchesRepository,
            IPreviewConfigurationsRepository previewConfigurationsRepository,
            IGithubReposStore githubReposStore
        )
        {
            _githubBranchesRepository = githubBranchesRepository;
            _previewConfigurationsRepository = previewConfigurationsRepository;
            _githubReposStore = githubReposStore;
        }

        protected override async Task HandleEventAsync(PushWebhookPayload pushEventPayload)
        {
            var repo = await _githubReposStore.GetByUriAsync(pushEventPayload.Repository.HtmlUrl);
            if (repo is null)
            {
                return;
            }
            var branchName = pushEventPayload.Ref.GetBranchNameFromFullRef();

            if (pushEventPayload.Created)
            {
                await _githubBranchesRepository.CreateAsync(repo, branchName, new GithubApiCommit
                {
                    CommitedOn = DateTime.Now,
                    Message = pushEventPayload.HeadCommit.Message,
                    Sha = pushEventPayload.After
                });
                return;
            }
            var existingBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(repo.Id, branchName);
            if (pushEventPayload.Deleted)
            {
                if (existingBranch == null)
                {
                    // branche déjà supprimée ou inexistante
                    return;
                }
                existingBranch.IsDeleted = true;
                existingBranch.DeletedAt = DateTime.Now;
                await _githubBranchesRepository.UpdateAsync(existingBranch);
                await _previewConfigurationsRepository.DeleteByBranchAsync(existingBranch);
            }
            else
            {
                if (existingBranch == null)
                {
                    // Branche inconnue
                    return;
                }
                existingBranch.HeadCommitHash = pushEventPayload.After;
                existingBranch.HeadCommitMessage = pushEventPayload.HeadCommit.Message;
                existingBranch.LastPushedAt = DateTime.Now;
                await _githubBranchesRepository.UpdateAsync(existingBranch);
            }
        }
    }
}
