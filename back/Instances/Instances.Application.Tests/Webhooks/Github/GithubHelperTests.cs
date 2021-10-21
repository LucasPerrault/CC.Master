using FluentAssertions;
using Instances.Application.Webhooks.Github;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Webhooks.Github
{
    public class GithubHelperTests
    {
        [Fact]
        public void GetBranchNameFromFullRefShouldHandleNullStringByReturningNull()
        {
            string nullRef = null;
            nullRef.GetBranchNameFromFullRef().Should().BeNull();
        }
        [Fact]
        public void GetBranchNameFromFullRefShouldHandleEmptyStringByReturningEmptyString()
        {
            var emptyRef = "";
            emptyRef.GetBranchNameFromFullRef().Should().BeEmpty();
        }
        [Fact]
        public void GetBranchNameFromFullRefShouldHandleOnlySpaceStringByReturningSameString()
        {
            var onlySpaceRef = "  ";
            onlySpaceRef.GetBranchNameFromFullRef().Should().Be(onlySpaceRef);
        }

        [Fact]
        public void GetBranchNameFromFullRefShouldHandleNormalFullRef()
        {
            var branchName = "master";
            var fullRef = $"refs/heads/{branchName}";
            fullRef.GetBranchNameFromFullRef().Should().Be(branchName);
        }

        [Fact]
        public void GetBranchNameFromFullRefShouldHandleBranchNamesWithSlash()
        {
            var branchName = "i/like/to/be/annoying";
            var fullRef = $"refs/heads/{branchName}";
            fullRef.GetBranchNameFromFullRef().Should().Be(branchName);
        }

        [Fact]
        public void GetBranchNameFromFullRefShouldNotThrowWhenFullRefIsNotPresent()
        {
            var branchName = "master";
            var @ref = $"{branchName}";
            @ref.GetBranchNameFromFullRef().Should().Be(@ref);
        }

        [Fact]
        public void GetBranchNameFromFullRefShouldNotThrowWhenFullRefIsNotPresent2()
        {
            var branchName = "master";
            var @ref = $"origin/{branchName}";
            @ref.GetBranchNameFromFullRef().Should().Be(@ref);
        }
    }
}
