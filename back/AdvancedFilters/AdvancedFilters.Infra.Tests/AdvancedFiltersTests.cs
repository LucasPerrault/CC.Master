using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters;
using FluentAssertions;
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
                Filter = new EnvironmentAdvancedCriterion().WithCountryId(ComparisonOperators.Equals, 250),
                Check = e => e.LegalUnits.Any(lu => lu.CountryId == 250)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithCountryId(ComparisonOperators.NotEquals, 250),
                Check = e => e.LegalUnits.Any(lu => lu.CountryId != 250)
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wpagga"),
                Check = e => e.AppInstances.Any(ai => ai.ApplicationId == "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.NotEquals, "wpagga"),
                Check = e => e.AppInstances.Any(ai => ai.ApplicationId != "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.Equals, "miaou")
                    .WithCountryId(ComparisonOperators.NotEquals, 250)
                    .WithApplicationId(ComparisonOperators.Equals, "wpagga"),
                Check = e =>
                    e.Subdomain == "miaou"
                    && e.LegalUnits.Any(lu => lu.CountryId != 250)
                    && e.AppInstances.Any(ai => ai.ApplicationId == "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion()
                    .WithSubdomain(ComparisonOperators.NotEquals, "miaou")
                    .WithCountryId(ComparisonOperators.Equals, 250)
                    .WithApplicationId(ComparisonOperators.NotEquals, "wpagga"),
                Check = e =>
                    e.Subdomain != "miaou"
                    && e.LegalUnits.Any(lu => lu.CountryId == 250)
                    && e.AppInstances.Any(ai => ai.ApplicationId != "wpagga")
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wfiggo")
                    .And(new EnvironmentAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou")),
                Check = e =>
                    e.AppInstances.Any(ai => ai.ApplicationId == "wfiggo")
                    && e.Subdomain != "miaou"
            }};
            yield return new object[] { new AdvancedFilterTestEntry<Environment>
            {
                Filter = new EnvironmentAdvancedCriterion().WithApplicationId(ComparisonOperators.Equals, "wfiggo")
                    .Or(new EnvironmentAdvancedCriterion().WithSubdomain(ComparisonOperators.NotEquals, "miaou")),
                Check = e =>
                    e.AppInstances.Any(ai => ai.ApplicationId == "wfiggo")
                    || e.Subdomain != "miaou"
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

        private IQueryable<Environment> GetEnvironments()
        {
            return new List<Environment>
            {
                new Environment
                {
                    Id = 1,
                    Subdomain = "miaou",
                    LegalUnits = GetLegalUnits(250, 42),
                    AppInstances = GetAppInstances("wfiggo", "wpagga")
                },
                new Environment
                {
                    Id = 2,
                    Subdomain = "ouaf",
                    LegalUnits = GetLegalUnits(250, 276),
                    AppInstances = GetAppInstances("wexpenses", "wpoplee")
                },
                new Environment
                {
                    Id = 3,
                    Subdomain = "wau",
                    LegalUnits = GetLegalUnits(276, 9001),
                    AppInstances = GetAppInstances("wfiggo")
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
        #endregion Environments

        #region AppContacts
        public static IEnumerable<object[]> GetAppContactTestData()
        {
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
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                    AppInstance = new AppInstance { ApplicationId = "wpagga" }
                },
                new AppContact
                {
                    Id = 2,
                    Environment = new Environment { Subdomain = "ouaf" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 9001 } },
                    AppInstance = new AppInstance { ApplicationId = "wexpenses" }
                },
                new AppContact
                {
                    Id = 3,
                    Environment = new Environment { Subdomain = "wau" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 276 } },
                    AppInstance = new AppInstance { ApplicationId = "wpoplee" }
                }
            }.AsQueryable();
        }
        #endregion AppContacts

        #region ClientContacts
        public static IEnumerable<object[]> GetClientContactTestData()
        {
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
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                },
                new ClientContact
                {
                    Id = 2,
                    Environment = new Environment { Subdomain = "ouaf" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 9001 } },
                },
                new ClientContact
                {
                    Id = 3,
                    Environment = new Environment { Subdomain = "wau" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 276 } },
                }
            }.AsQueryable();
        }
        #endregion ClientContacts

        #region SpecializedContacts
        public static IEnumerable<object[]> GetSpecializedContactTestData()
        {
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
                    Environment = new Environment { Subdomain = "miaou" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 250 } },
                },
                new SpecializedContact
                {
                    Id = 2,
                    Environment = new Environment { Subdomain = "ouaf" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 9001 } },
                },
                new SpecializedContact
                {
                    Id = 3,
                    Environment = new Environment { Subdomain = "wau" },
                    Establishment = new Establishment { LegalUnit = new LegalUnit { CountryId = 276 } },
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
            criterion.Subdomain = new SingleValueComparisonCriterion<string>
            {
                Operator = op,
                Value = subdomain
            };
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithCountryId(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnits = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleValueComparisonCriterion<int>
                {
                    Operator = op,
                    Value = countryId
                }
            };
            return criterion;
        }
        public static EnvironmentAdvancedCriterion WithApplicationId(this EnvironmentAdvancedCriterion criterion, ComparisonOperators op, string applicationId)
        {
            criterion.AppInstances = new AppInstanceAdvancedCriterion
            {
                ApplicationId = new SingleValueComparisonCriterion<string>
                {
                    Operator = op,
                    Value = applicationId
                }
            };
            return criterion;
        }

        public static AppContactAdvancedCriterion WithSubdomain(this AppContactAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Environment = new EnvironmentAdvancedCriterion
            {
                Subdomain = new SingleValueComparisonCriterion<string>
                {
                    Operator = op,
                    Value = subdomain
                }
            };
            return criterion;
        }
        public static AppContactAdvancedCriterion WithCountryId(this AppContactAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnit = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleValueComparisonCriterion<int>
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
                ApplicationId = new SingleValueComparisonCriterion<string>
                {
                    Operator = op,
                    Value = applicationId
                }
            };
            return criterion;
        }

        public static ClientContactAdvancedCriterion WithSubdomain(this ClientContactAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Environment = new EnvironmentAdvancedCriterion
            {
                Subdomain = new SingleValueComparisonCriterion<string>
                {
                    Operator = op,
                    Value = subdomain
                }
            };
            return criterion;
        }
        public static ClientContactAdvancedCriterion WithCountryId(this ClientContactAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnit = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleValueComparisonCriterion<int>
                {
                    Operator = op,
                    Value = countryId
                }
            };
            return criterion;
        }

        public static SpecializedContactAdvancedCriterion WithSubdomain(this SpecializedContactAdvancedCriterion criterion, ComparisonOperators op, string subdomain)
        {
            criterion.Environment = new EnvironmentAdvancedCriterion
            {
                Subdomain = new SingleValueComparisonCriterion<string>
                {
                    Operator = op,
                    Value = subdomain
                }
            };
            return criterion;
        }
        public static SpecializedContactAdvancedCriterion WithCountryId(this SpecializedContactAdvancedCriterion criterion, ComparisonOperators op, int countryId)
        {
            criterion.LegalUnit = new LegalUnitAdvancedCriterion
            {
                CountryId = new SingleValueComparisonCriterion<int>
                {
                    Operator = op,
                    Value = countryId
                }
            };
            return criterion;
        }
    }
}
