
using Instances.Domain.Instances.Models;
using System;
using Tools;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Domain.Instances
{
    public static class EnvironmentExtensions
    {
        public const string PreviewDomain = "ilucca-preview.net";
        public const string TrainingDomain = "ilucca-test.net";

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
                InstanceType.Training => TrainingDomain,
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
