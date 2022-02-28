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

        [Fact]
        public void ForProduction_AlwaysLowercaseDnsEntry()
        {
            var subDomain = "TEST";
            var cluster = "cluster1";
            var entry = DnsEntry.ForProduction(subDomain, cluster, EnvironmentDomain.ILuccaDotNet);

            Assert.Equal("test", entry.Subdomain);
        }
        #endregion

        #region ForDemo
        [Fact]
        public void ForDemo_AlwaysLowercaseDnsEntry()
        {
            var subDomain = "TEST";
            var cluster = "cluster1";
            var entry = DnsEntry.ForDemo(subDomain, cluster);

            Assert.Equal("test", entry.Subdomain);
        }
        #endregion

        #region ForPreview
        [Fact]
        public void ForPreview_AlwaysLowercaseDnsEntry()
        {
            var subDomain = "TEST";
            var cluster = "cluster1";
            var entry = DnsEntry.ForPreview(subDomain, cluster);

            Assert.Equal("test", entry.Subdomain);
        }
        #endregion

        #region ForTraining
        [Fact]
        public void ForTraining_AlwaysLowercaseDnsEntry()
        {
            var subDomain = "TEST";
            var cluster = "cluster1";
            var entry = DnsEntry.ForTraining(subDomain, cluster);

            Assert.Equal("test", entry.Subdomain);
        }
        #endregion

        #region ForRedirection
        [Fact]
        public void ForRedirection_AlwaysLowercaseDnsEntry()
        {
            var subDomain = "TEST";
            var entry = DnsEntry.ForRedirection(subDomain);

            Assert.Equal("test", entry.Subdomain);
        }
        #endregion
    }
}
