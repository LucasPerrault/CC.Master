
using Instances.Domain.Instances.Models;
using Instances.Domain.Trainings;
using System;
using Tools;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Domain.Instances
{
    public static class EnvironmentExtensions
    {
        private const string PreviewDomain = "ilucca-preview.net";

        public static string GetInstanceExecutingCluster(this CCEnvironment environment, InstanceType instanceType)
        {
            return instanceType switch
            {
                InstanceType.Prod => environment.Cluster,
                InstanceType.Training => Instance.TRAINING_EXECUTING_CLUSTER,
                InstanceType.Preview => Instance.PREVIEW_EXECUTING_CLUSTER,
                _ => throw new NotSupportedException()
            };
        }

        public static string GetInstanceDomain(this CCEnvironment environment, InstanceType instanceType)
        {
            return instanceType switch
            {
                InstanceType.Prod => environment.Domain.GetDescription(),
                InstanceType.Training => Training.TrainingDomain,
                InstanceType.Preview => PreviewDomain,
                _ => throw new NotSupportedException()
        };
        }

        public static string GetInstanceHost(this CCEnvironment environment, InstanceType instanceType)
        {
            return $"{environment.Subdomain}.{environment.GetInstanceDomain(instanceType)}";
        }

    }
}
