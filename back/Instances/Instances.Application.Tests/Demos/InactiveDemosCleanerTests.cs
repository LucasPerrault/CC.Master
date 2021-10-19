using Email.Domain;
using Instances.Application.Demos.Deletion;
using Instances.Application.Demos.Emails;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Moq;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace Instances.Application.Tests.Demos
{
    public class InactiveDemosCleanerTests
    {
        private readonly Mock<IDemosStore> _demosStoreMock;
        private readonly Mock<ICcDataService> _ccDataServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IInstanceSessionLogsService> _sessionLogsServiceMock;
        private readonly Mock<ITimeProvider> _timeProviderMock;
        private readonly Mock<IDemoEmails> _emailsMock;
        private readonly Mock<IInstancesStore> _instancesStoreMock;
        private readonly Mock<IDemoDeletionCalculator> _deletionCalculatorMock;
        private readonly Mock<IDnsService> _dnsServiceMock;

        public InactiveDemosCleanerTests()
        {
            _timeProviderMock = new Mock<ITimeProvider>();
            _sessionLogsServiceMock = new Mock<IInstanceSessionLogsService>();
            _demosStoreMock = new Mock<IDemosStore>();
            _ccDataServiceMock = new Mock<ICcDataService>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailsMock = new Mock<IDemoEmails>();
            _instancesStoreMock = new Mock<IInstancesStore>();
            _deletionCalculatorMock = new Mock<IDemoDeletionCalculator>();
            _dnsServiceMock = new Mock<IDnsService>();

            _emailsMock.Setup(e => e.GetIntentEmail(It.IsAny<DateTime>(), It.IsAny<IEnumerable<DemoCleanupInfo>>()))
                .Returns(new EmailContentBuilder("mocked"));
        }

        [Fact]
        public async Task CleanupShouldNotAffectTemplateOrProtectedDemos()
        {
            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2020, 01, 01));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object,
                _emailsMock.Object,
                _deletionCalculatorMock.Object,
                _instancesStoreMock.Object,
                _dnsServiceMock.Object
            );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});


            _demosStoreMock.Verify
            (
                s => s.GetAsync
                (
                    It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly
                        && f.IsTemplate == CompareBoolean.FalseOnly
                        && f.IsProtected == CompareBoolean.FalseOnly
                    ),
                    It.IsAny<AccessRight>()
                ),
                Times.Once()
            );
        }

        [Fact]
        public async Task CleanupShouldUpdateDeletedScheduledOnWhenDemoIsStandard()
        {

            var demo = new Demo
            {
                AuthorId = 440,
                Subdomain = "recently-used",
                Cluster = "mocked-demo-cluster",
                Instance = new Instance { IsProtected = false },
            };
            var demos = new List<Demo> { demo };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));
            _deletionCalculatorMock
                .Setup(c => c.GetDeletionDate(It.IsAny<Demo>(), It.IsAny<DateTime>()))
                .Returns(new DateTime(2010, 03, 04));

            _demosStoreMock
                .Setup(s => s.GetAsync
                (
                    It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly),
                    It.IsAny<AccessRight>()
                ))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object,
                _emailsMock.Object,
                _deletionCalculatorMock.Object,
                _instancesStoreMock.Object,
                _dnsServiceMock.Object
            );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});

            _demosStoreMock.Verify
            (
                s => s.UpdateDeletionScheduleAsync
                    (
                        It.Is<IEnumerable<DemoDeletionSchedule>>
                            (
                                schedules => schedules
                                    .Single(s => s.Demo == demo)
                                    .DeletionScheduledOn == new DateTime(2010, 03, 04)
                            )
                    )
            );
        }

        [Fact]
        public async Task CleanupShouldUpdateDeletedScheduledOnWhenDemoIsHubspot()
        {

            var demo = new Demo
            {
                AuthorId = 0,  // hubspot author id
                Subdomain = "recently-used",
                Cluster = "mocked-demo-cluster",
                Instance = new Instance { IsProtected = false },
            };
            var demos = new List<Demo> { demo };

            _timeProviderMock
                .Setup(p => p.Today())
                .Returns(new DateTime(2010, 10, 01));
            _deletionCalculatorMock
                .Setup(c => c.GetDeletionDate(It.IsAny<Demo>(), It.IsAny<DateTime>()))
                .Returns(new DateTime(2010, 02, 01));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object,
                _emailsMock.Object,
                _deletionCalculatorMock.Object,
                _instancesStoreMock.Object,
                _dnsServiceMock.Object
            );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});

            _demosStoreMock.Verify(s => s.UpdateDeletionScheduleAsync
            (
                It.Is<IEnumerable<DemoDeletionSchedule>>
                (
                    schedules => schedules
                    .Single(s => s.Demo == demo).DeletionScheduledOn == new DateTime(2010, 02, 01))
                )
            );
        }

        [Fact]
        public async Task CleanupShouldUpdateDeletedScheduledOnWhenDemoIsNeverUsed()
        {

            var demo = new Demo
            {
                AuthorId = 440,
                CreatedAt = new DateTime(2020, 01, 01),
                Subdomain = "recently-used",
                Cluster = "mocked-demo-cluster",
                Instance = new Instance { IsProtected = false },
            };
            var demos = new List<Demo> { demo };

            _timeProviderMock
                .Setup(p => p.Today())
                .Returns(new DateTime(0001, 01, 01));
            _deletionCalculatorMock
                .Setup(c => c.GetDeletionDate(It.IsAny<Demo>(), It.IsAny<DateTime>()))
                .Returns(new DateTime(2010, 03, 04));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object,
                _emailsMock.Object,
                _deletionCalculatorMock.Object,
                _instancesStoreMock.Object,
                _dnsServiceMock.Object
            );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});

            _demosStoreMock.Verify
            (
                s => s.UpdateDeletionScheduleAsync
                    (
                        It.Is<IEnumerable<DemoDeletionSchedule>>
                            (
                                schedules => schedules
                                    .Single(s => s.Demo == demo)
                                    .DeletionScheduledOn == new DateTime(2010, 03, 04)
                            )
                    )
            );
        }

        [Fact]
        public async Task CleanupShouldNotAffectBarelyDeletableDemos()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "recently-used",
                    Cluster = "mocked-demo-cluster",
                    Instance = new Instance { IsProtected = false },
                    DeletionScheduledOn = new DateTime(2010, 10, 02)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object,
                _emailsMock.Object,
                _deletionCalculatorMock.Object,
                _instancesStoreMock.Object,
                _dnsServiceMock.Object
            );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});

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
                    Cluster = "mocked-demo-cluster",
                    Instance = new Instance { IsProtected = false },
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object,
                    _emailsMock.Object,
                    _deletionCalculatorMock.Object,
                    _instancesStoreMock.Object,
                    _dnsServiceMock.Object
                );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    ItContainsSubdomains("inactive-for-too-long"),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ), Times.Once);

            _demosStoreMock.Verify(s => s.DeleteAsync(It.Is<IEnumerable<Demo>>(ds => ds.All(d => demos.Contains(d)))), Times.Once);
            _instancesStoreMock.Verify(s => s.DeleteForDemoAsync
                (
                    It.Is<IEnumerable<Instance>>(instances => instances.All(i => demos.Any(d => d.Instance == i)))
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task CleanupShouldNotRequestDeletionOfInactiveDemosDuringDryRun()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "inactive-for-too-long",
                    Cluster = "mocked-demo-cluster",
                    Instance = new Instance { IsProtected = false },
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
                (
                    _timeProviderMock.Object,
                    _sessionLogsServiceMock.Object,
                    _demosStoreMock.Object,
                    _ccDataServiceMock.Object,
                    _emailServiceMock.Object,
                    _emailsMock.Object,
                    _deletionCalculatorMock.Object,
                    _instancesStoreMock.Object,
                    _dnsServiceMock.Object
                );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = true});

            _ccDataServiceMock.Verify(s => s.DeleteInstancesAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ), Times.Never);

            _demosStoreMock.Verify(s => s.DeleteAsync(It.IsAny<IEnumerable<Demo>>()), Times.Never);
            _instancesStoreMock.Verify(s => s.DeleteForDemoAsync(It.IsAny<IEnumerable<Instance>>()), Times.Never);
        }

        [Fact]
        public async Task CleanupShouldRequestDeletionOfDemosByClusterBatch()
        {
            var demos = new List<Demo>
            {
                new Demo
                {
                    Subdomain = "d1-c1",
                    Cluster = "cluster-demo-1",
                    Instance = new Instance { IsProtected = false },
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                },
                new Demo
                {
                    Subdomain = "d2-c1",
                    Cluster = "cluster-demo-1",
                    Instance = new Instance { IsProtected = false },
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                },
                new Demo
                {
                    Subdomain = "d1-c2",
                    Cluster = "cluster-demo-2",
                    Instance = new Instance { IsProtected = false },
                    DeletionScheduledOn = new DateTime(2010, 10, 01)
                }
            };

            _timeProviderMock.Setup(p => p.Today()).Returns(new DateTime(2010, 10, 01));

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(f => f.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);

            var cleaner = new InactiveDemosCleaner
            (
                _timeProviderMock.Object,
                _sessionLogsServiceMock.Object,
                _demosStoreMock.Object,
                _ccDataServiceMock.Object,
                _emailServiceMock.Object,
                _emailsMock.Object,
                _deletionCalculatorMock.Object,
                _instancesStoreMock.Object,
                _dnsServiceMock.Object
            );

            await cleaner.CleanAsync(new DemoCleanupParams { IsDryRun = false});

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
