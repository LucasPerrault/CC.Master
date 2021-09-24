using Authentication.Domain.Helpers;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Trainings;
using System;
using System.Security.Claims;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Application.Trainings.Restoration
{
    public static class TrainingRestorationFactory
    {
        public static TrainingRestoration New
        (
            ClaimsPrincipal principal,
            CCEnvironment environment,
            bool anonymize,
            bool restoreFiles,
            bool keepExistingTrainingPasswords,
            string comment = "",
            DateTime? commentExpiryDate = null
        )
        {
            var instanceDuplication = new InstanceDuplication
            {
                Id = Guid.NewGuid(),
                SourceType = InstanceType.Prod,
                TargetType = InstanceType.Training,
                DistributorId = principal.GetDistributorId(),
                SourceCluster = environment.Cluster,
                TargetCluster = "TEST",
                SourceSubdomain = environment.Subdomain,
                TargetSubdomain = environment.Subdomain,
                Progress = InstanceDuplicationProgress.Pending
            };

            return new TrainingRestoration
            {
                EnvironmentId = environment.Id,
                AuthorId = principal.GetAuthorId(),
                ApiKeyStorableId = principal.GetApiKeyStorableId(),
                InstanceDuplicationId = instanceDuplication.Id,
                InstanceDuplication = instanceDuplication,
                Anonymize = anonymize,
                Comment = comment,
                CommentExpiryDate = commentExpiryDate,
                KeepExistingTrainingPasswords = keepExistingTrainingPasswords,
                RestoreFiles = restoreFiles,
            };
        }
    }
}
