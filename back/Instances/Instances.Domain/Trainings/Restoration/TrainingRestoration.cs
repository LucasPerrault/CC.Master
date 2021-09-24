using Instances.Domain.Instances;
using System;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Domain.Trainings
{
    public class TrainingRestoration
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string ApiKeyStorableId { get; set; }
        public Guid InstanceDuplicationId { get; set; }
        public int EnvironmentId { get; set; }
        public CCEnvironment Environment { get; set; }
        public string Comment { get; set; }
        public DateTime? CommentExpiryDate { get; set; }
        public bool Anonymize { get; set; }
        public bool RestoreFiles { get; set; }
        public bool KeepExistingTrainingPasswords { get; set; }
        public InstanceDuplication InstanceDuplication { get; set; }

        public int DistributorId
        {
            get
            {
                ThrowIfInstanceDuplicationIsNotIncluded();
                return InstanceDuplication.DistributorId;
            }
        }

        public bool HasEnded
        {
            get
            {
                ThrowIfInstanceDuplicationIsNotIncluded();
                return InstanceDuplication.EndedAt.HasValue;
            }
        }

        private void ThrowIfInstanceDuplicationIsNotIncluded()
        {
            if (InstanceDuplication == null)
            {
                throw new ApplicationException($"{nameof(TrainingRestoration)}.{nameof(InstanceDuplication)} was not included");
            }
        }
    }

    public class TrainingRestorationRequest
    {
        public int EnvironmentId { get; set; }
        public string Comment { get; set; }
        public DateTime? CommentExpiryDate { get; set; }
        public bool Anonymize { get; set; }
        public bool RestoreFiles { get; set; }
        public bool KeepExistingTrainingPasswords { get; set; }
    }
}
