using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Github
{
    public interface IGithubService
    {
        Task<string> GetFileContentAsync(string repoUrl, string filepath);
    }

    public class GithubService : IGithubService
    {
        private static readonly List<string> GithubAllowedOwners = new List<string> { "LuccaSA" };
        private const string GithubUrlBaseSecured = "https://github.com/";
        private const char UrlSeparator = '/';

        private readonly ILogger<GithubService> _logger;
        private readonly GitHubClient _gitHubClient;

        public GithubService(ILogger<GithubService> logger, GitHubClient gitHubClient)
        {
            _logger = logger;
            _gitHubClient = gitHubClient;
        }

        public async Task<string> GetFileContentAsync(string repoUrl, string filepath)
        {
            var (owner, repositoryName) = GetOwnerAndRepoNameFromRepoUrl(repoUrl);

            RepositoryContent file;
            try
            {
                var repoContent = await _gitHubClient.Repository.Content.GetAllContents(owner, repositoryName, filepath);
                file = repoContent.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Octokit failed to retrieve data from {owner}/{repositoryName} : {filepath}");
                throw new ApplicationException($"Error occurred while fetching github file : {e.Message}");
            }

            if (file == null || file.Type != ContentType.File)
            {

                throw new ApplicationException($"File {filepath} could not be found on repository {repositoryName}");
            }

            return file.Content;
        }

        private static (string owner, string repositoryName) GetOwnerAndRepoNameFromRepoUrl(string repoUrl)
        {
            if (repoUrl == null)
            {
                throw new BadRequestException("Veuillez préciser une adresse vers le repository ciblé.");
            }

            if (!repoUrl.StartsWith(GithubUrlBaseSecured))
            {
                throw new BadRequestException($"L'adresse vers le repository doit commencer par {GithubUrlBaseSecured}.");
            }

            var repoInformation = repoUrl.Substring(GithubUrlBaseSecured.Length).Split(UrlSeparator);

            if (repoInformation.Length != 2)
            {
                throw new BadRequestException($"L'adresse vers le repository doit être de la forme {GithubUrlBaseSecured}OWNER/REPO-NAME (reçu : {repoUrl}).");
            }

            var owner = repoInformation[0];

            if (GithubAllowedOwners.All(o => !string.Equals(o, owner, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new BadRequestException($"{owner} n'est pas un owner Github valide (autorisé : {string.Join(", ", GithubAllowedOwners)}) .");
            }

            var repositoryName = repoInformation[1];

            return (owner, repositoryName);
        }
    }
}
