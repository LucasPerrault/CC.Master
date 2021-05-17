using System.ComponentModel;
using Xunit;

namespace Environments.Domain.Tests
{
    public class EnvironmentTests
    {
        [Fact]
        public void ProductionHostReturnsIluccaDotNetForILuccaDotNetEnum()
        {
            var subdomain = "tintin";
            var environment = new Environment
            {
                Subdomain = subdomain,
                Domain = EnvironmentDomain.ILuccaDotNet,
                IsActive = true,
                Purpose = EnvironmentPurpose.Contractual,
            };

            Assert.Equal($"https://{subdomain}.ilucca.net", environment.ProductionHost);
        }

        [Fact]
        public void ProductionHostReturnsIluccaDotChForILuccaDotChEnum()
        {
            var subdomain = "tintin";
            var environment = new Environment
            {
                Subdomain = subdomain,
                Domain = EnvironmentDomain.ILuccaDotCh,
                IsActive = true,
                Purpose = EnvironmentPurpose.Contractual,
            };

            Assert.Equal($"https://{subdomain}.ilucca.ch", environment.ProductionHost);
        }

        [Fact]
        public void ProductionHostReturnsDauphineForDauphineEnum()
        {
            var subdomain = "conges";
            var environment = new Environment
            {
                Subdomain = subdomain,
                Domain = EnvironmentDomain.DauphineDotFr,
                IsActive = true,
                Purpose = EnvironmentPurpose.Contractual,
            };

            Assert.Equal($"https://{subdomain}.dauphine.fr", environment.ProductionHost);
        }


        [Fact]
        public void ProductionHostThrowsForUnknownEnumValue()
        {
            var subdomain = "conges";
            var environment = new Environment
            {
                Subdomain = subdomain,
                Domain = (EnvironmentDomain)int.MaxValue,
                IsActive = true,
                Purpose = EnvironmentPurpose.Contractual,
            };

            Assert.Throws<InvalidEnumArgumentException>(() => environment.ProductionHost);
        }
    }
}
