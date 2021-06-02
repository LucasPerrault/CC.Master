using FluentAssertions;
using Instances.Infra.Instances.Services;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using Xunit;

namespace Instances.Infra.Tests.Instances
{
    public class UserPasswordHelperTest
    {

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ThrowIfInvalid_ShouldThrowWithEmptyPassword(string password)
        {
            var userPasswordHelper = new UsersPasswordHelper();
            Assert.Throws<BadRequestException>(() => userPasswordHelper.ThrowIfInvalid(password));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("a2")]
        [InlineData("aB3")]
        [InlineData("aBc4")]
        [InlineData("aBcD5")]
        public void ThrowIfInvalid_ShouldThrowWithPasswordTooShort(string password)
        {
            var userPasswordHelper = new UsersPasswordHelper();
            Assert.Throws<BadRequestException>(() => userPasswordHelper.ThrowIfInvalid(password));
        }

        [Fact]
        public void ThrowIfInvalid_ShouldThrowWithPasswordTooLong()
        {
            var longPassword = "";
            for(var i = 0; i< 25; i++)
            {
                longPassword += "aBcDeFgHiJ";
            }
            longPassword += "xyz256";

            var userPasswordHelper = new UsersPasswordHelper();
            Assert.Throws<BadRequestException>(() => userPasswordHelper.ThrowIfInvalid(longPassword));
        }

        [Fact]
        public void ThrowIfInvalid_ShouldAcceptLegacyPassword()
        {
            var legacyPassword = "test";

            var userPasswordHelper = new UsersPasswordHelper();
            Action act = () => userPasswordHelper.ThrowIfInvalid(legacyPassword);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("abcdefgh!")]
        [InlineData("abcdefgh ")]
        [InlineData("abcdefgh)")]
        [InlineData("abcdefgh+")]
        [InlineData("abcdefgh.")]
        [InlineData("abcdefgh-")]
        [InlineData("abcdefgh$")]
        public void ThrowIfInvalid_ShouldThrowWithPasswordContaingSymbols(string password)
        {
            var userPasswordHelper = new UsersPasswordHelper();
            Assert.Throws<BadRequestException>(() => userPasswordHelper.ThrowIfInvalid(password));
        }

        [Fact]
        public void ThrowIfInvalid_ShouldNotThrowIfPasswordIsCorrect()
        {
            var correctPassword = "CorrectPassword1";
            var userPasswordHelper = new UsersPasswordHelper();
            Action act = () => userPasswordHelper.ThrowIfInvalid(correctPassword);
            act.Should().NotThrow();
        }
    }
}
