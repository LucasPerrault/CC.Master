using Email.Domain;
using Instances.Application.Demos;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace Instances.Application.Tests
{
    public class InactiveDemosCleanerTests
    {
        private readonly Mock<IDemosStore> _demosStoreMock;
        private readonly Mock<ICcDataService> _ccDataServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IInstanceSessionLogsService> _sessionLogsServiceMock;
        private readonly Mock<ITimeProvider> _timeProviderMock;

        public InactiveDemosCleanerTests()
        {
            _timeProviderMock = new Mock<ITimeProvider>();
            _sessionLogsServiceMock = new Mock<IInstanceSessionLogsService>();
            _demosStoreMock = new Mock<IDemosStore>();
            _ccDataServiceMock = new Mock<ICcDataService>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        [Fact]
        public async Task CleanupShouldNotAffectTemplateOrProtectedDemos()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "template",
                    IsTemplate = true,
                    DeletionScheduledOn = new DateTime(2020, 01 ,01)
                },
                new Demo
                {
                    Subdomain = "protected",
                    DeletionScheduledOn = new DateTime(2020, 01 ,01),
                    Instance = new Instance { IsProtected = true }
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2020, 01, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object
            );

            await cleaner.CleanAsync();

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ), Times.Never);
        }

        [Fact]
        public async Task CleanupShouldUpdateDeletedScheduledOnWhenDemoIsStandard()
        {

            var demo = new Demo
            {
                AuthorId = 440,
                Subdomain = "recently-used",
                Instance = new Instance { IsProtected = false, Cluster = "mocked-demo-cluster" },
            };
            var demos = new List<Demo> { demo };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            _sessionLogsServiceMock.Setup(s => s.GetLatestAsync(It.IsAny<Uri>()))
                .Returns(Task.FromResult(new DateTime(2010, 01, 01)));

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object
                );

            await cleaner.CleanAsync();
            _demosStoreMock.Verify(s => s.UpdateDeletionScheduleAsync(demo, new DateTime(2010, 03, 04)));
        }

        [Fact]
        public async Task CleanupShouldUpdateDeletedScheduledOnWhenDemoIsHubspot()
        {

            var demo = new Demo
            {
                AuthorId = 0,  // hubspot author id
                Subdomain = "recently-used",
                Instance = new Instance { IsProtected = false, Cluster = "mocked-demo-cluster" },
            };
            var demos = new List<Demo> { demo };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            _sessionLogsServiceMock.Setup(s => s.GetLatestAsync(It.IsAny<Uri>()))
                .Returns(Task.FromResult(new DateTime(2010, 01, 01)));

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object
                );

            await cleaner.CleanAsync();
            _demosStoreMock.Verify(s => s.UpdateDeletionScheduleAsync(demo, new DateTime(2010, 02, 01)));
        }

        [Fact]
        public async Task CleanupShouldUpdateDeletedScheduledOnWhenDemoIsNeverUsed()
        {

            var demo = new Demo
            {
                AuthorId = 440,
                CreatedAt = new DateTime(2020, 01, 01),
                Subdomain = "recently-used",
                Instance = new Instance { IsProtected = false, Cluster = "mocked-demo-cluster" },
            };
            var demos = new List<Demo> { demo };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(0001, 01, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            _sessionLogsServiceMock.Setup(s => s.GetLatestAsync(It.IsAny<Uri>()))
                .Returns(Task.FromResult(new DateTime(2010, 01, 01)));

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object
                );

            await cleaner.CleanAsync();
            _demosStoreMock.Verify(s => s.UpdateDeletionScheduleAsync(demo, new DateTime(2010, 03, 04)));
        }

        [Fact]
        public async Task CleanupShouldNotAffectBarelyDeletableDemos()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "recently-used",
                    Instance = new Instance { IsProtected = false, Cluster = "mocked-demo-cluster"},
                    DeletionScheduledOn = new DateTime(2010, 10, 02)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object
                );

            await cleaner.CleanAsync();

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ), Times.Never);
        }

        [Fact]
        public async Task CleanupShouldRequestDeletionOfInactiveDemos()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "inactive-for-too-long",
                    Instance = new Instance { IsProtected = false, Cluster = "mocked-demo-cluster"},
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object
                );

            await cleaner.CleanAsync();

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    ItContainsSubdomains("inactive-for-too-long"),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ), Times.Once);
        }

        [Fact]
        public async Task CleanupShouldRequestDeletionOfDemosByClusterBatch()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "d1-c1",
                    Instance = new Instance { IsProtected = false, Cluster = "cluster-demo-1"},
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                },
                new Demo
                {
                    Subdomain = "d2-c1",
                    Instance = new Instance { IsProtected = false, Cluster = "cluster-demo-1"},
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                },
                new Demo
                {
                    Subdomain = "d1-c2",
                    Instance = new Instance { IsProtected = false, Cluster = "cluster-demo-2"},
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetActiveAsync())
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));


            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object
                );

            await cleaner.CleanAsync();

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    ItContainsSubdomains("d1-c1", "d2-c1"),
                    It.Is<string>(cluster => cluster == "cluster-demo-1"),
                    It.IsAny<string>()
                ), Times.Once);

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    ItContainsSubdomains("d1-c2"),
                    It.Is<string>(cluster => cluster == "cluster-demo-2"),
                    It.IsAny<string>()
                ), Times.Once);
        }

        private IEnumerable<string> ItContainsSubdomains(params string[] subdomain)
        {
            return It.Is<IEnumerable<string>>
                (
                    e => subdomain.Select(e.Contains)
                        .Aggregate((a, b) => a && b)
                );
        }
    }
}
