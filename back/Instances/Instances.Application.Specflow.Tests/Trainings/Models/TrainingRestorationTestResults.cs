using Environments.Domain;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Instances.Domain.Trainings;
using Instances.Infra.Storage;
using System.Collections.Generic;
using Testing.Infra;
using Testing.Specflow;

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
        public List<string> TrainingCleaningScriptsUri { get; } = new List<string>();
        public List<string> AnonymizationScriptsUri { get; } = new List<string>();
    }
}
