using Environments.Domain;
using Instances.Domain.Shared;
using Instances.Domain.Trainings;
using System.Collections.Generic;

namespace Instances.Application.Specflow.Tests.Trainings.Models
{
    public class TrainingRestorationTestResults
    {
        public List<int> DeletedInstanceIds { get; } = new List<int>();
        public Environment EnvironmentToRestore { get; set; }
        public int? PreviousTrainingInstanceId { get; set; }
        public TrainingRestoration TrainingRestorationCreated { get; set; }
        public (CreateInstanceBackupRequestDto request, string targetCluster) BackupRequestParameters { get; set; }
        public (DuplicateInstanceRequestDto request, string targetCluster, string callbackUri) DuplicateRequestParameters { get; set; }
        public TrainingRestorationRequest TrainingRestorationRequest { get; set; }
        public List<System.Uri> TrainingCleaningScriptsUri { get; } = new List<System.Uri>();
        public List<System.Uri> AnonymizationScriptsUri { get; } = new List<System.Uri>();
    }
}
