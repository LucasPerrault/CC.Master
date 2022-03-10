using Environments.Domain;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Trainings;
using System;
using System.Collections.Generic;
using Tools;
using Xunit;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Domain.Tests.Instances
{
    public class EnvironmentExtensionsTests
    {
        [Fact]
        public void GetInstanceExecutingCluster_TestAllCases()
        {
            var env = new CCEnvironment
            {
                Cluster = "CLUSTERX"
            };

            Assert.Equal(env.Cluster, env.GetInstanceExecutingCluster(InstanceType.Prod));
            Assert.Equal(Instance.TRAINING_EXECUTING_CLUSTER, env.GetInstanceExecutingCluster(InstanceType.Training));
            Assert.Equal(Instance.PREVIEW_EXECUTING_CLUSTER, env.GetInstanceExecutingCluster(InstanceType.Preview));
            Assert.Throws<NotSupportedException>(() => env.GetInstanceExecutingCluster(InstanceType.Demo));
        }

        [Theory]
        [MemberData(nameof(EnvironmentDomainValues))]
        public void GetInstanceDomain_TestAllCases(EnvironmentDomain domain)
        {
            var env = new CCEnvironment
            {
                Domain = domain
            };

            Assert.Equal(domain.GetDescription(), env.GetInstanceDomain(InstanceType.Prod));
            Assert.Equal(Training.TrainingDomain, env.GetInstanceDomain(InstanceType.Training));
            Assert.Equal(EnvironmentExtensions.PreviewDomain, env.GetInstanceDomain(InstanceType.Preview));
            Assert.Throws<NotSupportedException>(() => env.GetInstanceDomain(InstanceType.Demo));
        }

        public static IEnumerable<object[]> EnvironmentDomainValues()
        {
            foreach (var number in Enum.GetValues(typeof(EnvironmentDomain)))
            {
                yield return new object[] { number };
            }
        }

        [Theory]
        [MemberData(nameof(EnvironmentDomainValues))]
        public void GetInstanceHost_TestAllCases(EnvironmentDomain domain)
        {
            var env = new CCEnvironment
            {
                Domain = domain,
                Subdomain = "babar",
            };
            Assert.DoesNotContain("://", env.GetInstanceHost(InstanceType.Prod));
            Assert.StartsWith(env.Subdomain, env.GetInstanceHost(InstanceType.Prod));
            Assert.Contains(domain.GetDescription(), env.GetInstanceHost(InstanceType.Prod));

            Assert.DoesNotContain("://", env.GetInstanceHost(InstanceType.Training));
            Assert.StartsWith(env.Subdomain, env.GetInstanceHost(InstanceType.Training));
            Assert.Contains(Training.TrainingDomain, env.GetInstanceHost(InstanceType.Training));

            Assert.DoesNotContain("://", env.GetInstanceHost(InstanceType.Preview));
            Assert.StartsWith(env.Subdomain, env.GetInstanceHost(InstanceType.Preview));
            Assert.Contains(EnvironmentExtensions.PreviewDomain, env.GetInstanceHost(InstanceType.Preview));

            Assert.Throws<NotSupportedException>(() => env.GetInstanceHost(InstanceType.Demo));
        }
    }
}
