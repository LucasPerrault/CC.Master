using Environments.Domain;
using FluentAssertions;
using Instances.Domain.Instances;
using System;
using Xunit;

namespace Instances.Domain.Tests.Instances
{
    public class DnsEntryTests
    {

        #region ForProduction
        [Theory]
        [InlineData(EnvironmentDomain.ILuccaDotNet, DnsEntryZone.RbxProductions)]
        [InlineData(EnvironmentDomain.ILuccaDotCh, DnsEntryZone.ChProductions)]
        public void ForProduction_ValidDomain(EnvironmentDomain environmentDomain, DnsEntryZone zone)
        {
            var subDomain = "test";
            var cluster = "cluster1";
            var entry = DnsEntry.ForProduction(subDomain, cluster, environmentDomain);

            entry.Zone.Should().Be(zone);
        }

        [Theory]
        [InlineData(EnvironmentDomain.DauphineDotFr)]
        [InlineData((EnvironmentDomain)42)]
        public void ForProduction_InvalidDomain(EnvironmentDomain environmentDomain)
        {
            var subDomain = "test";
            var cluster = "cluster1";
            Action act = () => DnsEntry.ForProduction(subDomain, cluster, environmentDomain);

            act.Should().Throw<NotSupportedException>();
        }

        #endregion
    }
}
