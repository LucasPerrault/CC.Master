using FluentAssertions;
using FluentAssertions.Json;
using Instances.Domain.Shared;
using Instances.Infra.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Shared
{
    public class ClusterNameConvertorTests
    {

        public ClusterNameConvertorTests()
        {
        }

        [Theory]
        [InlineData("green")]
        [InlineData("green3")]
        public void GetShortName_DoesNotGiveNumberForGreen(string clusterName)
        {
            Assert.DoesNotMatch(".*\\d.*", ClusterNameConvertor.GetShortName(clusterName));
        }

        [Theory]
        [InlineData("demo")]
        [InlineData("demo2")]
        [InlineData("demo1")]
        public void GetShortName_ShouldAlwaysReturnANumberForDemo(string clusterName)
        {
            Assert.Matches(".*\\d$", ClusterNameConvertor.GetShortName(clusterName));
        }

        [Theory]
        [InlineData("cluster")]
        [InlineData("demo")]
        [InlineData("preview")]
        [InlineData("formation")]
        [InlineData("green")]
        [InlineData("security")]
        [InlineData("recette")]
        public void GetShortName_ShouldHandleKnownClusterNames(string clusterName)
        {
            Action act = () => ClusterNameConvertor.GetShortName(clusterName);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData("babar")]
        [InlineData("tintin")]
        public void GetShortName_ShouldThrowWithUnknownClusterNames(string clusterName)
        {
            Action act = () => ClusterNameConvertor.GetShortName(clusterName);
            act.Should().Throw<NotSupportedException>();
        }

    }
}
