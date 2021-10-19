using Authentication.Domain;
using Distributors.Domain.Models;
using Environments.Domain;
using Environments.Domain.Storage;
using FluentAssertions;
using Lucca.Core.Api.Abstractions.Paging;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Collections.Generic;
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

        private static Page<Environment> ToPage(List<Environment> envs) => new Page<Environment> { Items = envs };

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
                .Setup(e => e.GetAsync(It.IsAny<IPageToken>(), It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(ToPage(new List<Environment> { environment }));

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(null, FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>()
            };

            accesses.Items.Should().Equals(expected);
        }

        [Fact]
        public async Task ShouldReturnSimpleAccess()
        {
            var distributor = new Distributor { Code = "APERTURE" };
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Consumer = distributor,
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    }
                }
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<IPageToken>(), It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(ToPage(new List<Environment> { environment }));

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(null, FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>
                {
                    new DistributorWithAccess(distributor, new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract })
                }
            };

            accesses.Items.Should().Equal(expected);
        }

        [Fact]
        public async Task ShouldReturnAccessWithTwoTypes()
        {
            var distributor = new Distributor { Code = "APERTURE" };

            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Consumer = distributor,
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    },
                    new EnvironmentSharedAccess
                    {
                        Consumer = distributor,
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Manual }
                    },
                }
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<IPageToken>(), It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(ToPage(new List<Environment> { environment }));

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(null, FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>
                {
                    new DistributorWithAccess(distributor, new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract, EnvironmentAccessTypeEnum.Manual })
                }
            };

            accesses.Items.Should().Equal(expected);
        }

        [Fact]
        public async Task ShouldReturnTwoAccesses()
        {
            var mesa = new Distributor { Code = "MESA" };
            var aperture = new Distributor { Code = "APERTURE" };
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "aperture-science",
                ActiveAccesses = new List<EnvironmentSharedAccess>
                {
                    new EnvironmentSharedAccess
                    {
                        Consumer = aperture,
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    },
                    new EnvironmentSharedAccess
                    {
                        Consumer = mesa,
                        Access = new EnvironmentAccess {Type = EnvironmentAccessTypeEnum.Contract }
                    },
                }
            };

            _envStoreMock
                .Setup(e => e.GetAsync(It.IsAny<IPageToken>(), It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(ToPage(new List<Environment> { environment }));

            var repository = NewRepository();
            var accesses = await repository.GetAccessesAsync(null, FilterBySubdomain("aperture-science"));

            var expected = new EnvironmentWithAccess
            {
                Id = 42,
                Subdomain = "aperture-science",
                Accesses = new HashSet<DistributorWithAccess>
                {
                    new DistributorWithAccess(aperture, new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract }),
                    new DistributorWithAccess(mesa, new List<EnvironmentAccessTypeEnum> { EnvironmentAccessTypeEnum.Contract }),
                }
            };

            accesses.Items.Should().Equal(expected);
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
