using Authentication.Domain;
using Distributors.Domain.Models;
using Environments.Domain;
using Environments.Domain.Storage;
using FluentAssertions;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Users.Domain;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Environments.Application.Tests
{
    public class EnvironmentsRepositoryTests
    {
        private readonly Mock<IEnvironmentsStore> _envStoreMock = new Mock<IEnvironmentsStore>();
        private readonly Mock<IRightsService> _rightServiceMock = new Mock<IRightsService>();

        public EnvironmentsRepositoryTests()
        {
            _rightServiceMock
                .Setup(s => s.GetScopedPermissionsAsync(Operation.ReadEnvironments))
                .ReturnsAsync(new List<ScopedPermission>());
        }

        [Fact]
        public async Task ShouldThrowWhenNotFound()
        {

            var environment = new Environment
            {
                Id = 24,
                Subdomain = "black-mesa",
                ActiveAccesses = new List<EnvironmentSharedAccess>()
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(FilterBySubdomain("aperture-science"));
            accesses.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldReturnNothingAccess()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>()
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment> { environment });

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>()
            };

            accesses.Should().Equals(expected);
        }

        [Fact]
        public async Task ShouldReturnSimpleAccess()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Consumer = new Distributor { Code = "APERTURE" },
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    }
                }
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment> { environment });

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>
                {
                    new DistributorWithAccess("APERTURE", new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract })
                }
            };

            accesses.Should().Equal(expected);
        }

        [Fact]
        public async Task ShouldReturnAccessWithTwoTypes()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Consumer = new Distributor { Code = "APERTURE" },
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    },
                    new EnvironmentSharedAccess
                    {
                        Consumer = new Distributor { Code = "APERTURE" },
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Manual }
                    },
                }
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment> { environment });

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>
                {
                    new DistributorWithAccess("APERTURE", new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract, EnvironmentAccessTypeEnum.Manual })
                }
            };

            accesses.Should().Equal(expected);
        }

        [Fact]
        public async Task ShouldReturnTwoAccesses()
        {
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Consumer = new Distributor { Code = "APERTURE" },
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    },
                    new EnvironmentSharedAccess
                    {
                        Consumer = new Distributor { Code = "MESA" },
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    },
                }
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment> { environment });

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>
                {
                    new DistributorWithAccess("APERTURE", new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract }),
                    new DistributorWithAccess("MESA", new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract }),
                }
            };

            accesses.Should().Equal(expected);
        }

        private EnvironmentsRepository NewRepository()
        {
            var principal = new CloudControlUserClaimsPrincipal(new Principal
            {
                UserId = 1,
                User = new User { FirstName = "Gla", LastName = "DOS" }
            });

            return new EnvironmentsRepository(_envStoreMock.Object, principal, new EnvironmentRightsFilter(_rightServiceMock.Object));
        }

        private EnvironmentFilter FilterBySubdomain(string subdomain) => new EnvironmentFilter { Subdomain = CompareString.Equals(subdomain)};
    }
}
