using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Domain.Instances.Models;
using Instances.Infra.Instances.Services;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Instances.Infra.Tests.Storage.Stores
{
    public class InstancesStoreTests
    {
        private readonly InstancesDbContext _instancesDbContext;
        private readonly Mock<IQueryPager> _queryPagerMock;
        private readonly InstancesStore _instancesStore;
        private readonly Mock<IInstancesRemoteStore> _instancesRemoteStoreMock;

        public InstancesStoreTests()
        {
            _instancesDbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
            _queryPagerMock = new Mock<IQueryPager>();
            _queryPagerMock
                .Setup(p => p.ToPageAsync(It.IsAny<IQueryable<CodeSource>>(), It.IsAny<IPageToken>()))
                .Returns<IQueryable<CodeSource>, IPageToken>(
                    (queryable, pageToken) => Task.FromResult(new Page<CodeSource> { Items = queryable.ToList() })
                );
            _instancesRemoteStoreMock = new Mock<IInstancesRemoteStore>();

            _instancesStore = new InstancesStore(_instancesDbContext, _queryPagerMock.Object, _instancesRemoteStoreMock.Object);
        }

        #region GetActiveInstanceFromEnvironmentId
        [Fact]
        public void GetActiveInstanceFromEnvironmentId_DoesNotHandleDemos()
        {
            Assert.Throws<BadRequestException>(() => _instancesStore.GetActiveInstanceFromEnvironmentId(1, InstanceType.Demo));
        }


        [Theory]
        [MemberData(nameof(InstanceTypeValuesExceptDemo))]
        public async Task GetActiveInstanceFromEnvironmentId_ReturnsOnlyInstancesOfCorrectType(InstanceType instanceType)
        {
            var environmentId = 2;
            await _instancesDbContext.AddAsync(new Instance { Id = 1, EnvironmentId = null, IsActive = true, Type = InstanceType.Demo });
            await _instancesDbContext.AddAsync(new Instance { Id = 2, EnvironmentId = environmentId, IsActive = true, Type = InstanceType.Prod });
            await _instancesDbContext.AddAsync(new Instance { Id = 3, EnvironmentId = environmentId, IsActive = true, Type = InstanceType.Training });
            await _instancesDbContext.AddAsync(new Instance { Id = 4, EnvironmentId = environmentId, IsActive = true, Type = InstanceType.Preview });
            await _instancesDbContext.SaveChangesAsync();

            var instance = _instancesStore.GetActiveInstanceFromEnvironmentId(environmentId, instanceType);
            Assert.Equal(instanceType, instance.Type);
        }

        [Fact]
        public async Task GetActiveInstanceFromEnvironmentId_ReturnsOnlyAnActiveInstance()
        {
            var environmentId = 2;
            var instanceType = InstanceType.Training;
            await _instancesDbContext.AddAsync(new Instance { Id = 2, EnvironmentId = environmentId, IsActive = false, Type = instanceType });
            await _instancesDbContext.AddAsync(new Instance { Id = 3, EnvironmentId = environmentId, IsActive = true, Type = instanceType });
            await _instancesDbContext.AddAsync(new Instance { Id = 4, EnvironmentId = environmentId, IsActive = false, Type = instanceType });
            await _instancesDbContext.SaveChangesAsync();

            var instance = _instancesStore.GetActiveInstanceFromEnvironmentId(environmentId, instanceType);
            Assert.True(instance.IsActive);
        }


        [Fact]
        public async Task GetActiveInstanceFromEnvironmentId_ReturnsAnInstanceFromTheCorrectEnvironment()
        {
            var environmentId = 2;
            var instanceType = InstanceType.Training;
            await _instancesDbContext.AddAsync(new Instance { Id = 2, EnvironmentId = 1, IsActive = true, Type = instanceType });
            await _instancesDbContext.AddAsync(new Instance { Id = 3, EnvironmentId = environmentId, IsActive = true, Type = instanceType });
            await _instancesDbContext.AddAsync(new Instance { Id = 4, EnvironmentId = 4, IsActive = true, Type = instanceType });
            await _instancesDbContext.SaveChangesAsync();

            var instance = _instancesStore.GetActiveInstanceFromEnvironmentId(environmentId, instanceType);
            Assert.Equal(environmentId, instance.EnvironmentId);
        }


        public static IEnumerable<object[]> InstanceTypeValuesExceptDemo()
        {
            foreach (var number in Enum.GetValues(typeof(InstanceType)))
            {
                if((InstanceType)number != InstanceType.Demo)
                {
                    yield return new object[] { number };
                }
            }
        }
        #endregion

        #region CreateForTrainingAsync
        [Fact]
        public async Task CreateForTrainingAsync_DelegatesToRemoteStore()
        {
            _instancesRemoteStoreMock.Setup(irs => irs.CreateForTrainingAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new Instance { Id = 1, Type = InstanceType.Training });

            var environmentId = 1;
            var isAnonymized = true;
            await _instancesStore.CreateForTrainingAsync(environmentId, isAnonymized);

            _instancesRemoteStoreMock.Verify(irs => irs.CreateForTrainingAsync(environmentId, isAnonymized), Times.Once);
        }
        #endregion

        #region CreateForDemoAsync
        [Fact]
        public async Task CreateForDemoAsync_DelegatesToRemoteStore()
        {
            _instancesRemoteStoreMock.Setup(irs => irs.CreateForDemoAsync(It.IsAny<string>())).ReturnsAsync(new Instance { Id = 1, Type = InstanceType.Demo });

            var password = "tintin";
            await _instancesStore.CreateForDemoAsync(password);

            _instancesRemoteStoreMock.Verify(irs => irs.CreateForDemoAsync(password), Times.Once);
        }
        #endregion

        #region CreateForTrainingAsync
        [Fact]
        public async Task DeleteByIdAsync_DelegatesToRemoteStore()
        {
            _instancesRemoteStoreMock.Setup(irs => irs.DeleteByIdAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var instanceId = 1;
            await _instancesStore.DeleteByIdAsync(instanceId);

            _instancesRemoteStoreMock.Verify(irs => irs.DeleteByIdAsync(instanceId), Times.Once);
        }
        #endregion

        #region CreateForTrainingAsync
        [Fact]
        public async Task DeleteByIdsAsync_DelegatesToRemoteStore()
        {
            _instancesRemoteStoreMock.Setup(irs => irs.DeleteByIdsAsync(It.IsAny<IEnumerable<int>>())).Returns(Task.CompletedTask);

            var instanceIds = new List<int> { 1, 2 };
            await _instancesStore.DeleteByIdsAsync(instanceIds);

            _instancesRemoteStoreMock.Verify(irs => irs.DeleteByIdsAsync(instanceIds), Times.Once);
        }
        #endregion
    }
}
