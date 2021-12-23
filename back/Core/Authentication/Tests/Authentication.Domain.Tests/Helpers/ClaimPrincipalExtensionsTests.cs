using Authentication.Domain.Helpers;
using System;
using Users.Domain;
using Xunit;

namespace Authentication.Domain.Tests.Helpers
{
    public class ClaimPrincipalExtensionsTests
    {
        #region GetAuthorIdOnlyWhenUser
        [Fact]
        public void GetAuthorIdOnlyWhenUser_ShouldReturnNullWhenClaimsPrincipalIsNotAUser()
        {
            var cp = new CloudControlApiKeyClaimsPrincipal(new ApiKey()
            {
                Name = "api-key",
                Token = Guid.Empty,
            });

            Assert.Null(cp.GetAuthorIdOnlyWhenUser());
        }

        [Fact]
        public void GetAuthorIdOnlyWhenUser_ShouldReturnTheUserIdWhenClaimsPrincipalIsAUser()
        {
            var userId = 2;
            var cp = new CloudControlUserClaimsPrincipal(new Principal()
            {
                Token = Guid.Empty,
                UserId = userId,
                User = new User()
                {
                    Id = userId,
                    DistributorId = 3,
                    DepartmentId = 4,
                    FirstName = "Jean",
                    LastName = "Bombeur",
                    LegalEntityId = 5,
                    Login = "jbombeur",
                    Mail = "jbombeur@example.org",
                    ManagerId = 6,
                }
            });

            Assert.Equal(userId, cp.GetAuthorIdOnlyWhenUser());
        }
        #endregion

        #region GetAuthorId
        [Fact]
        public void GetAuthorId_ShouldReturn0WhenClaimsPrincipalIsNotAUser()
        {
            var cp = new CloudControlApiKeyClaimsPrincipal(new ApiKey()
            {
                Name = "api-key",
                Token = Guid.Empty,
            });

            Assert.Equal(0, cp.GetAuthorId());
        }

        [Fact]
        public void GetAuthorId_ShouldReturnTheUserIdWhenClaimsPrincipalIsAUser()
        {
            var userId = 2;
            var cp = new CloudControlUserClaimsPrincipal(new Principal()
            {
                Token = Guid.Empty,
                UserId = userId,
                User = new User()
                {
                    Id = userId,
                    DistributorId = 3,
                    DepartmentId = 4,
                    FirstName = "Jean",
                    LastName = "Bombeur",
                    LegalEntityId = 5,
                    Login = "jbombeur",
                    Mail = "jbombeur@example.org",
                    ManagerId = 6,
                }
            });

            Assert.Equal(userId, cp.GetAuthorId());
        }
        #endregion

        #region GetApiKeyStorableId
        [Fact]
        public void GetApiKeyStorableId_ShouldNotReturnNullOrEmptyStringWhenClaimsPrincipalIsAnApiKey()
        {
            var cp = new CloudControlApiKeyClaimsPrincipal(new ApiKey()
            {
                Name = "api-key",
                Token = Guid.Empty,
            });

            Assert.False(string.IsNullOrEmpty(cp.GetApiKeyStorableId()));
        }

        [Fact]
        public void GetApiKeyStorableId_ShouldNotReturnTheFullTokenWhenClaimsPrincipalIsAnApiKey()
        {
            var token = Guid.Empty;
            var cp = new CloudControlApiKeyClaimsPrincipal(new ApiKey()
            {
                Name = "api-key",
                Token = token,
            });

            var tokenAsString = token.ToString();

            var storableId = cp.GetApiKeyStorableId();
            Assert.NotEqual(tokenAsString, storableId);
            Assert.DoesNotContain(tokenAsString, storableId);
        }

        [Fact]
        public void GetApiKeyStorableId_ShouldReturnNullWhenClaimsPrincipalIsAUser()
        {
            var cp = new CloudControlUserClaimsPrincipal(new Principal()
            {
                Token = Guid.Empty,
                UserId = 2,
                User = new User()
                {
                    Id = 2,
                    DistributorId = 3,
                    DepartmentId = 4,
                    FirstName = "Jean",
                    LastName = "Bombeur",
                    LegalEntityId = 5,
                    Login = "jbombeur",
                    Mail = "jbombeur@example.org",
                    ManagerId = 6,
                }
            });

            Assert.Null(cp.GetApiKeyStorableId());
        }
        #endregion
    }
}
