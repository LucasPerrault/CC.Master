using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Webhooks.Github
{
    public class PushWebhookService : GithubWebhookBaseService<PushWebhookPayload>
    {
        private readonly ICodeSourcesRepository _codeSourcesRepository;
        private readonly IGithubBranchesRepository _githubBranchesRepository;
        private readonly IPreviewConfigurationsRepository _previewConfigurationsRepository;

        public PushWebhookService(
            ICodeSourcesRepository codeSourcesRepository, IGithubBranchesRepository githubBranchesRepository,
            IPreviewConfigurationsRepository previewConfigurationsRepository)
        {
            _codeSourcesRepository = codeSourcesRepository;
            _githubBranchesRepository = githubBranchesRepository;
            _previewConfigurationsRepository = previewConfigurationsRepository;
        }

        protected override async Task HandleEventAsync(PushWebhookPayload pushEventPayload)
        {
            var codeSources = await _codeSourcesRepository.GetNonDeletedByRepositoryUrlAsync(pushEventPayload.Repository.HtmlUrl);
            if (codeSources.Count == 0)
            {
                return;
            }
            var firstCodeSource = codeSources.First();

            var branchName = pushEventPayload.Ref.GetBranchNameFromFullRef();
            var existingBranch = await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(firstCodeSource, branchName);

            if (pushEventPayload.Created)
            {
                if (existingBranch != null)
                {
                    throw new BadRequestException($"Branche {existingBranch.Name} déjà existante pour le repo {firstCodeSource.GithubRepo} alors qu'on souhaite la créer");
                }
                var branch = new GithubBranch()
                {
                    CodeSources = codeSources,
                    Name = branchName,
                    HeadCommitHash = pushEventPayload.After,
                    HeadCommitMessage = pushEventPayload.HeadCommit.Message,
                    LastPushedAt = DateTime.Now
                };
                branch = await _githubBranchesRepository.CreateAsync(branch);
                await _previewConfigurationsRepository.CreateByBranchAsync(branch);
            }
            else if (pushEventPayload.Deleted)
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
