using Instances.Application.CodeSources;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.DataDuplication
{
    public class SqlScriptPicker : ISqlScriptPicker
    {
        private const int PeopleSphereDistirbutorId = 36;
        private const string CommonSqlScriptKeyword = "common";
        private readonly ICodeSourcesRepository _codeSourcesRepository;

        public SqlScriptPicker(ICodeSourcesRepository codeSourcesRepository)
        {
            _codeSourcesRepository = codeSourcesRepository;
        }

        public async Task<List<Uri>> GetForAnonymizationAsync(InstanceDuplication duplication)
        {
            return (await _codeSourcesRepository.GetInstanceAnonymizationArtifactsAsync()).Select(a => new Uri(a.ArtifactUrl)).ToList();
        }

        public async Task<List<Uri>> GetForCleaningAsync(InstanceDuplication duplication)
        {
            var instanceTypeAsString = (duplication.TargetType == InstanceType.Demo ? InstanceType.Training : duplication.TargetType).ToString().ToLowerInvariant();

            var artifacts = await _codeSourcesRepository.GetInstanceCleaningArtifactsAsync();
            return artifacts
                .Where(a => a.FileName.Contains(CommonSqlScriptKeyword) || a.FileName.Contains(instanceTypeAsString))
                .Select(a => new Uri(a.ArtifactUrl)).ToList();

        }

        public async Task<List<Uri>> GetExtraForDuplicationAsync(InstanceDuplication duplication)
        {
            var result = new List<Uri>();
            var monolithArtifacts = await _codeSourcesRepository.GetMonolithArtifactsAsync();

            if (duplication.TargetType == InstanceType.Demo && duplication.DistributorId == PeopleSphereDistirbutorId)
            {
                var mprh = monolithArtifacts.FirstOrDefault(a => a.FileName == "MPRH_Demo.login.uniqueness.sql");
                if(mprh != null)
                {
                    result.Add(new Uri(mprh.ArtifactUrl));
                }
            }
            if (duplication.TargetType == InstanceType.Prod)
            {
                var newInstance = monolithArtifacts.FirstOrDefault(a => a.FileName == "clean_db_for_new_instance.sql");
                if (newInstance != null)
                {
                    result.Add(new Uri(newInstance.ArtifactUrl));
                }
            }
            return result;
        }

        public async Task<List<Uri>> GetPreRestoreScriptsAsync(InstanceDuplication duplication, InstanceDuplicationOptions duplicationOptions)
        {
            var instanceTypeAsString = duplication.TargetType.ToString().ToLowerInvariant();

            var artifacts = (await _codeSourcesRepository.GetInstancePreRestoreArtifactsAsync()).ToList();

            return duplicationOptions.SpecificPreRestoreScriptKeywordSelector.SelectMany(keyword =>
                    artifacts.Where(a => a.FileName.Contains($"{keyword}.{CommonSqlScriptKeyword}") || a.FileName.Contains($"{keyword}.{instanceTypeAsString}")))
                .Select(a => new Uri(a.ArtifactUrl)).ToList();
        }

        public async Task<List<Uri>> GetPostRestoreScriptsAsync(InstanceDuplication duplication, InstanceDuplicationOptions duplicationOptions)
        {
            var instanceTypeAsString = duplication.TargetType.ToString().ToLowerInvariant();

            var artifacts = (await _codeSourcesRepository.GetInstancePostRestoreArtifactsAsync()).ToList();

            return duplicationOptions.SpecificPostRestoreScriptKeywordSelector.SelectMany(keyword =>
                    artifacts.Where(a => a.FileName.Contains($"{keyword}.{CommonSqlScriptKeyword}") || a.FileName.Contains($"{keyword}.{instanceTypeAsString}")))
                .Select(a => new Uri(a.ArtifactUrl)).ToList();
        }
    }
}
