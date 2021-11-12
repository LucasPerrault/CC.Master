using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters;
using AdvancedFilters.Infra.Filters.Builders.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Tests
{
    public class AdvancedFiltersTests
    {
        #region Environments
        public static IEnumerable<object[]> GetEnvironmentTestData()
        {
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithSubdomain(ComparisonOperators.Equals, "miaou"),
                Check = e => e.Subdomain == "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou"),
                Check = e => e.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCluster(ComparisonOperators.Equals, "c1"),
                Check = e => e.Cluster == "c1"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCluster(ComparisonOperators.NotEquals, "c1"),
                Check = e => e.Cluster != "c1"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCreatedAt(ComparisonOperators.StrictlyGreaterThan, new DateTime(2020, 01, 01)),
                Check = e => e.CreatedAt > new DateTime(2020, 01, 01)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCreatedAt(ComparisonOperators.StrictlyLessThan, new DateTime(2020, 01, 01)),
                Check = e => e.CreatedAt < new DateTime(2020, 01, 01)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCountryId(ComparisonOperators.Equals, 250, ItemsMatching.Any),
                Check = e => e.LegalUnits.Any(lu => lu.CountryId == 250)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCountryId(ComparisonOperators.NotEquals, 250, ItemsMatching.Any),
                Check = e => e.LegalUnits.Any(lu => lu.CountryId != 250)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wpagga", ItemsMatching.Any),
                Check = e => e.AppInstances.Any(ai => ai.ApplicationId == "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.NotEquals, "wpagga", ItemsMatching.Any),
                Check = e => e.AppInstances.Any(ai => ai.ApplicationId != "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.Equals, "miaou")
                    .WithCountryId(ComparisonOperators.NotEquals, 250, ItemsMatching.Any)
                    .WithApplicationId(ComparisonOperators.Equals, "wpagga", ItemsMatching.Any),
                Check = e =>
                    e.Subdomain == "miaou"
                    && e.LegalUnits.Any(lu => lu.CountryId != 250)
                    && e.AppInstances.Any(ai => ai.ApplicationId == "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.NotEquals, "miaou")
                    .WithCountryId(ComparisonOperators.Equals, 250, ItemsMatching.Any)
                    .WithApplicationId(ComparisonOperators.NotEquals, "wpagga", ItemsMatching.Any),
                Check = e =>
                    e.Subdomain != "miaou"
                    && e.LegalUnits.Any(lu => lu.CountryId == 250)
                    && e.AppInstances.Any(ai => ai.ApplicationId != "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wfiggo", ItemsMatching.Any)
                    .And(new EnvironmentAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou")),
                Check = e =>
                    e.AppInstances.Any(ai => ai.ApplicationId == "wfiggo")
                    && e.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wfiggo", ItemsMatching.Any)
                    .Or(new EnvironmentAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou")),
                Check = e =>
                    e.AppInstances.Any(ai => ai.ApplicationId == "wfiggo")
                    || e.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wfiggo", ItemsMatching.All),
                Check = e =>
                    e.AppInstances.All(ai => ai.ApplicationId == "wfiggo")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.NotEquals, "wfiggo", ItemsMatching.All),
                Check = e =>
                    e.AppInstances.All(ai => ai.ApplicationId != "wfiggo")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithDistributorType(ComparisonOperators.Equals, DistributorType.Direct),
                Check = e =>
                    e.Accesses.All(a => a.Type != EnvironmentAccessType.Contract || a.DistributorId == Environment.LuccaDistributorId)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithDistributorType(ComparisonOperators.Equals, DistributorType.Indirect),
                Check = e =>
                    e.Accesses.Any(a => a.Type == EnvironmentAccessType.Contract && a.DistributorId != Environment.LuccaDistributorId)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithDistributorType(ComparisonOperators.NotEquals, DistributorType.Direct),
                Check = e =>
                    e.Accesses.Any(a => a.Type == EnvironmentAccessType.Contract && a.DistributorId != Environment.LuccaDistributorId)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithDistributorType(ComparisonOperators.NotEquals, DistributorType.Indirect),
                Check = e =>
                    e.Accesses.All(a => a.Type != EnvironmentAccessType.Contract || a.DistributorId == Environment.LuccaDistributorId)
            }};
        }

        [Theory]
        [MemberData(nameof(GetEnvironmentTestData))]
        public void Environments_ShouldBeFoundBy_Search(AdvancedFilterTestEntry<Environment> testEntry)
        {
            var searchResult = GetEnvironments().Filter(testEntry.Filter);

            searchResult.Should().NotBeEmpty();
            searchResult.Should().OnlyContain(testEntry.Check);
        }

        [Fact]
        public void Environments_Search_ShouldFail_WhenItemsMatchedNotSpecified()
        {
            var appCriterion = new EnvironmentAdvancedCriterion()
                .WithApplicationId(It.IsAny<ComparisonOperators>(), It.IsAny<string>(), null);
            Func<IQueryable<Environment>> appFilterFn = () => GetEnvironments().Filter(appCriterion);

            appFilterFn.Should().ThrowExactly<MissingItemsMatchedFieldException<AppInstance>>();

            var luCriterion = new EnvironmentAdvancedCriterion()
                .WithApplicationId(It.IsAny<ComparisonOperators>(), It.IsAny<string>(), null);
            Func<IQueryable<Environment>> luFilterFn = () => GetEnvironments().Filter(luCriterion);

            luFilterFn.Should().ThrowExactly<MissingItemsMatchedFieldException<AppInstance>>();
        }

        private IQueryable<Environment> GetEnvironments()
        {
            return new List<Environment>
            {
                new Environment
                {
                    Id = 1,
                    CreatedAt = new DateTime(2021, 03, 01),
                    Subdomain = "miaou",
                    Cluster = "c2",
                    LegalUnits = GetLegalUnits(250, 42),
                    AppInstances = GetAppInstances("wfiggo", "wpagga"),
                    Accesses = GetAccesses(1, (Environment.LuccaDistributorId, EnvironmentAccessType.Contract))
                },
                new Environment
                {
                    Id = 2,
                    CreatedAt = new DateTime(2002, 03, 01),
                    Subdomain = "ouaf",
                    Cluster = "c1",
                    LegalUnits = GetLegalUnits(250, 276),
                    AppInstances = GetAppInstances("wexpenses", "wpoplee"),
                    Accesses = GetAccesses(2, (Environment.LuccaDistributorId, EnvironmentAccessType.Contract), (42, EnvironmentAccessType.Manual))
                },
                new Environment
                {
                    Id = 3,
                    CreatedAt = new DateTime(2002, 03, 01),
                    Subdomain = "wau",
                    Cluster = "c2",
                    LegalUnits = GetLegalUnits(276, 9001),
                    AppInstances = GetAppInstances("wfiggo"),
                    Accesses = GetAccesses(3, (Environment.LuccaDistributorId, EnvironmentAccessType.Contract), (42, EnvironmentAccessType.Contract))
                },
                new Environment
                {
                    Id = 4,
                    CreatedAt = new DateTime(2002, 03, 01),
                    Subdomain = "wau",
                    Cluster = "c2",
                    LegalUnits = GetLegalUnits(276, 9001),
                    AppInstances = GetAppInstances("wfiggo"),
                    Accesses = GetAccesses(4, (42, EnvironmentAccessType.Contract))
                }
            }.AsQueryable();
        }

        private IEnumerable<LegalUnit> GetLegalUnits(params int[] countryIds)
        {
            return countryIds.Select(id => new LegalUnit { CountryId = id }).ToList();
        }
        private IEnumerable<AppInstance> GetAppInstances(params string[] applicationIds)
        {
            return applicationIds.Select(id => new AppInstance { ApplicationId = id }).ToList();
        }
        private IEnumerable<EnvironmentAccess> GetAccesses(int envId, params (int DistributorId, EnvironmentAccessType AccessType)[] accesses)
        {
            return accesses.Select(access => new EnvironmentAccess { EnvironmentId = envId, DistributorId = access.DistributorId, Type = access.AccessType }).ToList();
        }
        #endregion Environments

        #region AppContacts
        public static IEnumerable<object[]> GetAppContactTestData()
        {
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithIsConfirmed(ComparisonOperators.Equals, true),
                Check = c => c.IsConfirmed
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithIsConfirmed(ComparisonOperators.NotEquals, true),
                Check = c => !c.IsConfirmed
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithSubdomain(ComparisonOperators.Equals, "miaou"),
                Check = c => c.Environment.Subdomain == "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou"),
                Check = c => c.Environment.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithCountryId(ComparisonOperators.Equals, 250),
                Check = c => c.Establishment.LegalUnit.CountryId == 250
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithCountryId(ComparisonOperators.NotEquals, 250),
                Check = c => c.Establishment.LegalUnit.CountryId != 250
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wpagga"),
                Check = c => c.AppInstance.ApplicationId == "wpagga"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion().WithApplicationId(ComparisonOperators.NotEquals, "wpagga"),
                Check = c => c.AppInstance.ApplicationId != "wpagga"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.Equals, "miaou")
                    .WithCountryId(ComparisonOperators.NotEquals, 9001)
                    .WithApplicationId(ComparisonOperators.Equals, "wpagga"),
                Check = c =>
                    c.Environment.Subdomain == "miaou"
                    && c.Establishment.LegalUnit.CountryId != 9001
                    && c.AppInstance.ApplicationId == "wpagga"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.NotEquals, "miaou")
                    .WithCountryId(ComparisonOperators.Equals, 276)
                    .WithApplicationId(ComparisonOperators.NotEquals, "wpagga"),
                Check = c =>
                    c.Environment.Subdomain != "miaou"
                    && c.Establishment.LegalUnit.CountryId == 276
                    && c.AppInstance.ApplicationId != "wpagga"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<AppContact>
            {
                Filter = new AppContactAdvancedCriterion
                {
                    Environment = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.NotEquals, "wpagga", ItemsMatching.All)
                },
                Check = c => c.Environment.AppInstances.All(i => i.ApplicationId != "wpagga")
            }};
        }

        [Theory]
        [MemberData(nameof(GetAppContactTestData))]
        public void AppContacts_ShouldBeFoundBy_Search(AdvancedFilterTestEntry<AppContact> testEntry)
        {
            var searchResult = GetAppContacts().Filter(testEntry.Filter);

            searchResult.Should().NotBeEmpty();
            searchResult.Should().OnlyContain(testEntry.Check);
        }

        private IQueryable<AppContact> GetAppContacts()
        {
            return new List<AppContact>
            {
                new AppContact
                {
                    Id = 1,
                    IsConfirmed = true,
                    Environment = new Environment { Subdomain = "miaou", AppInstances = new List<AppInstance> { new AppInstance { ApplicationId = "wpagga" } } },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                    AppInstance = new AppInstance { ApplicationId = "wpagga" }
                },
                new AppContact
                {
                    Id = 2,
                    IsConfirmed = true,
                    Environment = new Environment { Subdomain = "ouaf", AppInstances = new List<AppInstance> { new AppInstance { ApplicationId = "wexpenses" }, new AppInstance { ApplicationId = "wtimmi" } } },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 9001 } },
                    AppInstance = new AppInstance { ApplicationId = "wexpenses" }
                },
                new AppContact
                {
                    Id = 3,
                    IsConfirmed = true,
                    Environment = new Environment { Subdomain = "wau", AppInstances = new List<AppInstance> { new AppInstance { ApplicationId = "wexpenses" }, new AppInstance { ApplicationId = "wtimmi" } } },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 276 } },
                    AppInstance = new AppInstance { ApplicationId = "wpoplee" }
                },
                new AppContact
                {
                    Id = 4,
                    IsConfirmed = false,
                    Environment = new Environment { Subdomain = "miaou", AppInstances = new List<AppInstance> { new AppInstance { ApplicationId = "wpagga" } } },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                    AppInstance = new AppInstance { ApplicationId = "wpagga" }
                },
                new AppContact
                {
                    Id = 5,
                    IsConfirmed = true,
                    Environment = new Environment { Subdomain = "miaou", AppInstances = new List<AppInstance> { new AppInstance { ApplicationId = "wpagga" } } },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                    AppInstance = new AppInstance { ApplicationId = "wpagga" },
                }
            }.AsQueryable();
        }
        #endregion AppContacts

        #region ClientContacts
        public static IEnumerable<object[]> GetClientContactTestData()
        {
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithIsConfirmed(ComparisonOperators.Equals, true),
                Check = c => c.IsConfirmed
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithIsConfirmed(ComparisonOperators.NotEquals, true),
                Check = c => !c.IsConfirmed
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithClientId(ComparisonOperators.Equals, new Guid("6FBBA9BF-546A-4F46-B3C2-E18D73172A88")),
                Check = c => c.ClientId == new Guid("6FBBA9BF-546A-4F46-B3C2-E18D73172A88")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithClientId(ComparisonOperators.NotEquals, new Guid("6FBBA9BF-546A-4F46-B3C2-E18D73172A88")),
                Check = c => c.ClientId != new Guid("6FBBA9BF-546A-4F46-B3C2-E18D73172A88")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithSubdomain(ComparisonOperators.Equals, "miaou"),
                Check = c => c.Environment.Subdomain == "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou"),
                Check = c => c.Environment.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithCountryId(ComparisonOperators.Equals, 250),
                Check = c => c.Establishment.LegalUnit.CountryId == 250
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion().WithCountryId(ComparisonOperators.NotEquals, 250),
                Check = c => c.Establishment.LegalUnit.CountryId != 250
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.Equals, "miaou")
                    .WithCountryId(ComparisonOperators.NotEquals, 9001),
                Check = c =>
                    c.Environment.Subdomain == "miaou"
                    && c.Establishment.LegalUnit.CountryId != 9001
            }};
            yield return new object[] { new AdvancedFilterTestEntry<ClientContact>
            {
                Filter = new ClientContactAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.NotEquals, "miaou")
                    .WithCountryId(ComparisonOperators.Equals, 9001),
                Check = c =>
                    c.Environment.Subdomain != "miaou"
                    && c.Establishment.LegalUnit.CountryId == 9001
            }};
        }

        [Theory]
        [MemberData(nameof(GetClientContactTestData))]
        public void ClientContacts_ShouldBeFoundBy_Search(AdvancedFilterTestEntry<ClientContact> testEntry)
        {
            var searchResult = GetClientContacts().Filter(testEntry.Filter);

            searchResult.Should().NotBeEmpty();
            searchResult.Should().OnlyContain(testEntry.Check);
        }

        private IQueryable<ClientContact> GetClientContacts()
        {
            return new List<ClientContact>
            {
                new ClientContact
                {
                    Id = 1,
                    IsConfirmed = true,
                    ClientId = new Guid("6FBBA9BF-546A-4F46-B3C2-E18D73172A88"),
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                },
                new ClientContact
                {
                    Id = 2,
                    IsConfirmed = true,
                    ClientId = new Guid("DEADBEEF-546A-4F46-B3C2-E18D73172A88"),
                    Environment = new Environment { Subdomain = "ouaf" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 9001 } },
                },
                new ClientContact
                {
                    Id = 3,
                    IsConfirmed = true,
                    ClientId = new Guid("6FBBA9BF-546A-4F46-B3C2-E18D73172A88"),
                    Environment = new Environment { Subdomain = "wau" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 276 } },
                },
                new ClientContact
                {
                    Id = 4,
                    IsConfirmed = false,
                    ClientId = new Guid("DEADBEEF-546A-4F46-B3C2-E18D73172A88"),
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                }
            }.AsQueryable();
        }
        #endregion ClientContacts

        #region SpecializedContacts
        public static IEnumerable<object[]> GetSpecializedContactTestData()
        {
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithIsConfirmed(ComparisonOperators.Equals, true),
                Check = c => c.IsConfirmed
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithIsConfirmed(ComparisonOperators.NotEquals, true),
                Check = c => !c.IsConfirmed
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithRoleCode(ComparisonOperators.Equals, "dpo"),
                Check = c => c.RoleCode == "dpo"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithRoleCode(ComparisonOperators.NotEquals, "dpo"),
                Check = c => c.RoleCode != "dpo"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithSubdomain(ComparisonOperators.Equals, "miaou"),
                Check = c => c.Environment.Subdomain == "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou"),
                Check = c => c.Environment.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithCountryId(ComparisonOperators.Equals, 250),
                Check = c => c.Establishment.LegalUnit.CountryId == 250
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion().WithCountryId(ComparisonOperators.NotEquals, 250),
                Check = c => c.Establishment.LegalUnit.CountryId != 250
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.Equals, "miaou")
                    .WithCountryId(ComparisonOperators.NotEquals, 9001),
                Check = c =>
                    c.Environment.Subdomain == "miaou"
                    && c.Establishment.LegalUnit.CountryId != 9001
            }};
            yield return new object[] { new AdvancedFilterTestEntry<SpecializedContact>
            {
                Filter = new SpecializedContactAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.NotEquals, "miaou")
                    .WithCountryId(ComparisonOperators.Equals, 9001),
                Check = c =>
                    c.Environment.Subdomain != "miaou"
                    && c.Establishment.LegalUnit.CountryId == 9001
            }};
        }

        [Theory]
        [MemberData(nameof(GetSpecializedContactTestData))]
        public void SpecializedContacts_ShouldBeFoundBy_Search(AdvancedFilterTestEntry<SpecializedContact> testEntry)
        {
            var searchResult = GetSpecializedContacts().Filter(testEntry.Filter);

            searchResult.Should().NotBeEmpty();
            searchResult.Should().OnlyContain(testEntry.Check);
        }

        private IQueryable<SpecializedContact> GetSpecializedContacts()
        {
            return new List<SpecializedContact>
            {
                new SpecializedContact
                {
                    Id = 1,
                    IsConfirmed = true,
                    RoleCode = "security",
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                },
                new SpecializedContact
                {
                    Id = 2,
                    IsConfirmed = true,
                    RoleCode = "dpo",
                    Environment = new Environment { Subdomain = "ouaf" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 9001 } },
                },
                new SpecializedContact
                {
                    Id = 3,
                    IsConfirmed = true,
                    RoleCode = "general",
                    Environment = new Environment { Subdomain = "wau" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 276 } },
                },
                new SpecializedContact
                {
                    Id = 4,
                    IsConfirmed = false,
                    RoleCode = "security",
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                }
            }.AsQueryable();
        }
        #endregion SpecializedContacts
    }

    public class AdvancedFilterTestEntry<T>
    {
        public IAdvancedFilter Filter { get; set; }
        public Expression<Func<T, bool>> Check { get;set; }
    }

    internal static class AdvancedFilterExtensions
    {
        public static IAdvancedFilter And(this IAdvancedFilter filter1, IAdvancedFilter filter2) => filter1.Combine(FilterOperatorTypes.And, filter2);
        public static IAdvancedFilter Or(this IAdvancedFilter filter1, IAdvancedFilter filter2) => filter1.Combine(FilterOperatorTypes.Or, filter2);
        private static IAdvancedFilter Combine(this IAdvancedFilter filter1, FilterOperatorTypes op, IAdvancedFilter filter2)
        {
            return new FilterCombination
            {
                Operator = op,
                Values = new List<IAdvancedFilter> { filter1, filter2 }
            };
        }

        public static EnvironmentAdvancedCriterion WithSubdomain(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Subdomain = new SingleStringComparisonCriterion
            {
                Operator = op,
                Value = subdomain
            };
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithCluster(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, string cluster)
        {
            criterion.Cluster = new SingleStringComparisonCriterion
            {
                Operator = op,
                Value = cluster
            };
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithCreatedAt(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, DateTime createdAt)
        {
            criterion.CreatedAt = new SingleDateTimeComparisonCriterion
            {
                Operator = op,
                Value = createdAt
            };
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithDistributorType(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, DistributorType distributorType)
        {
            criterion.DistributorType = new SingleEnumComparisonCriterion<DistributorType>
            {
                Operator = op,
                Value = distributorType
            };
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithCountryId(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, int countryId, ItemsMatching? matching)
        {
            criterion.LegalUnits = new LegalUnitsAdvancedCriterion
            {
                CountryId = new SingleIntComparisonCriterion
                {
                    Operator = op,
                    Value = countryId
                },
            };
            if (matching.HasValue)
            {
                criterion.LegalUnits.ItemsMatched = matching.Value;
            }
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithApplicationId(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, string applicationId, ItemsMatching? matching)
        {
            criterion.AppInstances = new AppInstancesAdvancedCriterion
            {
                ApplicationId = new SingleStringComparisonCriterion
                {
                    Operator = op,
                    Value = applicationId
                },
            };
            if (matching.HasValue)
            {
                criterion.AppInstances.ItemsMatched = matching.Value;
            }
            return criterion;
        }

        public static AppContactAdvancedCriterion WithIsConfirmed(this AppContactAdvancedCriterion criterion, ComparisonOperators op, bool isConfirmed)
        {
            criterion.IsConfirmed = new SingleBooleanComparisonCriterion
            {
                Operator = op,
                Value = isConfirmed
            };
            return criterion;
        }
        public static AppContactAdvancedCriterion WithSubdomain(this AppContactAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Environment = new EnvironmentAdvancedCriterion().WithSubdomain(op, subdomain);

            return criterion;
        }
        public static AppContactAdvancedCriterion WithCountryId(this AppContactAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnit = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleIntComparisonCriterion
                {
                    Operator = op,
                    Value = countryId
                }
            };
            return criterion;
        }
        public static AppContactAdvancedCriterion WithApplicationId(this AppContactAdvancedCriterion criterion, ComparisonOperators op, string applicationId)
        {
            criterion.AppInstance = new AppInstanceAdvancedCriterion
            {
                ApplicationId = new SingleStringComparisonCriterion
                {
                    Operator = op,
                    Value = applicationId
                }
            };
            return criterion;
        }

        public static ClientContactAdvancedCriterion WithIsConfirmed(this ClientContactAdvancedCriterion criterion, ComparisonOperators op, bool isConfirmed)
        {
            criterion.IsConfirmed = new SingleBooleanComparisonCriterion
            {
                Operator = op,
                Value = isConfirmed
            };
            return criterion;
        }
        public static ClientContactAdvancedCriterion WithClientId(this ClientContactAdvancedCriterion criterion, ComparisonOperators op, Guid clientId)
        {
            criterion.ClientId = new SingleGuidComparisonCriterion
            {
                Operator = op,
                Value = clientId
            };
            return criterion;
        }
        public static ClientContactAdvancedCriterion WithSubdomain(this ClientContactAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Environment = new EnvironmentAdvancedCriterion().WithSubdomain(op, subdomain);

            return criterion;
        }
        public static ClientContactAdvancedCriterion WithCountryId(this ClientContactAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnit = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleIntComparisonCriterion
                {
                    Operator = op,
                    Value = countryId
                }
            };
            return criterion;
        }

        public static SpecializedContactAdvancedCriterion WithIsConfirmed(this SpecializedContactAdvancedCriterion criterion, ComparisonOperators op, bool isConfirmed)
        {
            criterion.IsConfirmed = new SingleBooleanComparisonCriterion
            {
                Operator = op,
                Value = isConfirmed
            };
            return criterion;
        }
        public static SpecializedContactAdvancedCriterion WithRoleCode(this SpecializedContactAdvancedCriterion criterion, ComparisonOperators op, string roleCode)
        {
            criterion.RoleCode = new SingleStringComparisonCriterion
            {
                Operator = op,
                Value = roleCode
            };
            return criterion;
        }
        public static SpecializedContactAdvancedCriterion WithSubdomain(this SpecializedContactAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Environment = new EnvironmentAdvancedCriterion().WithSubdomain(op, subdomain);

            return criterion;
        }
        public static SpecializedContactAdvancedCriterion WithCountryId(this SpecializedContactAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnit = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleIntComparisonCriterion
                {
                    Operator = op,
                    Value = countryId
                }
            };
            return criterion;
        }
    }
}
