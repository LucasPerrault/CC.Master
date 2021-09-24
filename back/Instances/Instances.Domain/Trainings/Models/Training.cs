using Instances.Domain.Instances.Models;
using System;
using Users.Domain;
using CCEnvironment= Environments.Domain.Environment;

namespace Instances.Domain.Trainings
{
    public class Training
    {
        public const string TrainingDomain = "ilucca-test.net";

        public int Id { get; set; }

        public int EnvironmentId { get; set; }
        public CCEnvironment Environment { get; set; }

        public int InstanceId { get; set; }
        public Instance Instance { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastRestoredAt { get; set; }

        public int AuthorId { get; set; }
        public SimpleUser Author { get; set; }
        public string ApiKeyStorableId { get; set; }
        public int TrainingRestorationId { get; set; }
        public TrainingRestoration TrainingRestoration { get; set; }

        public Uri Href => new Uri($"https://{Environment.Subdomain}.{TrainingDomain}");
    }
}
