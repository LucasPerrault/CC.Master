using Authentication.Domain;
using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Exceptions;
using Billing.Contracts.Infra.Storage;
using Billing.Contracts.Infra.Storage.Stores;
using Distributors.Domain.Models;
using FluentAssertions;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using Salesforce.Domain.Interfaces;
using Salesforce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Infra;
using Users.Domain;
using Xunit;

namespace Billing.Contracts.Application.Tests
{
    public class ClientsRepositoryTests
    {
        private readonly ContractsDbContext _dbContext = InMemoryDbHelper.InitialiseDb<ContractsDbContext>("contracts", o => new ContractsDbContext(o));
        private readonly Mock<IRightsService> _rightServiceMock = new Mock<IRightsService>(MockBehavior.Strict);
        private readonly Mock<ISalesforceAccountsRemoteService> _salesforceAccountRemoteServiceMock = new Mock<ISalesforceAccountsRemoteService>();
        private readonly Mock<ILegacyClientsRemoteService> _legacyClientServiceMock = new Mock<ILegacyClientsRemoteService>();

        private static readonly Distributor Partner = new Distributor
        {
            Id = 1, Code = "PAR", Name = "Partner"
        };
        private static readonly Distributor Lucca = new Distributor
        {
            Id = 2, Code = "LUC", Name = "Lucca"
        };

        [Fact]
        public async Task GetAsync_ShouldFindClientOutOfDistributor_WhenRightsAllowIt()
        {
            _rightServiceMock
                .Setup(r => r.GetUserOperationHighestScopeAsync(Operation.ReadContracts))
                .ReturnsAsync(() => AccessRightScope.AllDistributors);

            var client = new Client { Id = 1000, Contracts = new List<Contract> { GetContract(2, Partner) } };

            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var repo = new ClientsRepository
            (
                new ClientsStore(_dbContext),
                _legacyClientServiceMock.Object,
                new ClientRightFilter(new RightsFilter(_rightServiceMock.Object)),
                GetUserPrincipal(Lucca),
                _salesforceAccountRemoteServiceMock.Object
            );

            var clients = await repo.GetAsync();
            clients.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAsync_ShouldNotFindClientOutOfDistributor_WhenRightsDoNotAllowIt()
        {
            _rightServiceMock
                .Setup(r => r.GetUserOperationHighestScopeAsync(Operation.ReadContracts))
                .ReturnsAsync(() => AccessRightScope.OwnDistributorOnly);


            var client = new Client { Id = 1000, Contracts = new List<Contract> { GetContract(2, Partner) } };

            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var repo = new ClientsRepository
            (
                new ClientsStore(_dbContext),
                _legacyClientServiceMock.Object,
                new ClientRightFilter(new RightsFilter(_rightServiceMock.Object)),
                GetUserPrincipal(Lucca),
                _salesforceAccountRemoteServiceMock.Object
            );

            var clients = await repo.GetAsync();
            clients.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAsync_ShouldFindClientOfDistributor()
        {
            _rightServiceMock
                .Setup(r => r.GetUserOperationHighestScopeAsync(Operation.ReadContracts))
                .ReturnsAsync(() => AccessRightScope.OwnDistributorOnly);


            var client = new Client { Id = 1000, Contracts = new List<Contract> { GetContract(2, Partner) } };

            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var repo = new ClientsRepository
            (
                new ClientsStore(_dbContext),
                _legacyClientServiceMock.Object,
                new ClientRightFilter(new RightsFilter(_rightServiceMock.Object)),
                GetUserPrincipal(Partner),
                _salesforceAccountRemoteServiceMock.Object
            );

            var clients = await repo.GetAsync();
            clients.Should().HaveCount(1);
        }

        [Fact]
        public async Task PutAsync_ShouldNotAccessClient_WhenSubdomainIsNotMatching()
        {
            var client = new Client
            {
                Id = 1000,
                ExternalId = Guid.NewGuid(),
                Contracts = new List<Contract> { GetContract(2, Partner, "aperture-science") }
            };

            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var repo = new ClientsRepository
            (
                new ClientsStore(_dbContext),
                _legacyClientServiceMock.Object,
                new ClientRightFilter(new RightsFilter(_rightServiceMock.Object)),
                null,
                _salesforceAccountRemoteServiceMock.Object
            );

            await Assert.ThrowsAsync<ClientNotVisibleException>(() => repo.PutAsync(client.ExternalId, client, "black-mesa"));
        }

        [Fact]
        public async Task PutAsync_ShouldNotAccessClient_WhenIdIsNotMatching()
        {
            var client = new Client
            {
                Id = 1000,
                ExternalId = Guid.NewGuid(),
                Contracts = new List<Contract> { GetContract(2, Partner, "aperture-science") }
            };

            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var repo = new ClientsRepository
            (
                new ClientsStore(_dbContext),
                _legacyClientServiceMock.Object,
                new ClientRightFilter(new RightsFilter(_rightServiceMock.Object)),
                null,
                _salesforceAccountRemoteServiceMock.Object
            );

            await Assert.ThrowsAsync<ClientNotVisibleException>(() => repo.PutAsync(Guid.NewGuid(), client, "aperture-science"));
        }

        [Fact]
        public async Task PutAsync_ShouldWork_WhenDataIsMatching()
        {
            var client = new Client
            {
                Id = 1000,
                ExternalId = Guid.NewGuid(),
                SalesforceId = "SalesforceId",
                Contracts = new List<Contract> { GetContract(2, Partner, "aperture-science") }
            };

            await _dbContext.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var repo = new ClientsRepository
            (
                new ClientsStore(_dbContext),
                _legacyClientServiceMock.Object,
                new ClientRightFilter(new RightsFilter(_rightServiceMock.Object)),
                null,
                _salesforceAccountRemoteServiceMock.Object
            );

            var updatedClient = new Client
            {
                Id = client.Id,
                ExternalId = client.ExternalId,
                BillingCity = "Ajaccio",
            };

            await repo.PutAsync(client.ExternalId, updatedClient, "aperture-science");

            _salesforceAccountRemoteServiceMock
                .Verify(s => s.UpdateAccountAsync("SalesforceId", It.Is<SalesforceAccount>(a => a.BillingCity == "Ajaccio")), Times.Once);

            _legacyClientServiceMock
                .Verify(s => s.SyncAsync(), Times.Once);
        }

        private static CloudControlUserClaimsPrincipal GetUserPrincipal(Distributor distributor)
        {
            return new CloudControlUserClaimsPrincipal
            (
                new Principal
                {
                    Token = Guid.NewGuid(),
                    UserId = 1,
                    User = new User
                    {
                        Id = 1,
                        DepartmentId = 1,
                        LegalEntityId = 1,
                        DistributorCode = distributor.Code,
                        ManagerId = 0,
                    }
                }
            );
        }

        private static Contract GetContract(int id, Distributor distributor, string subdomain = "")
        {
            return new Contract
            {
                Id = id,
                Distributor = distributor,
                DistributorId = distributor.Id,
                EnvironmentSubdomain = subdomain
            };
        }

    }
}
