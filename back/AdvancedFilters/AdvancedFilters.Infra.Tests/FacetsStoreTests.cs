using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Facets.DAO;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Storage;
using AdvancedFilters.Infra.Storage.Stores;
using FluentAssertions;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Tests
{
    public class FacetsStoreTests
    {
        private AdvancedFiltersDbContext _dbContext { get; }
        public FacetsStoreTests()
        {
            _dbContext = InMemoryDbHelper.InitialiseDb<AdvancedFiltersDbContext>(new Guid().ToString(), o => new AdvancedFiltersDbContext(o));
            PopulateDatabase();
        }

        [Theory]
        [MemberData(nameof(GetEnvironmentFacetTestData))]
        public void ShouldReturnFilteredEnvironment(AdvancedEnvironmentFacetFilterEmptyResultTestEntry entry)
        {
            var sut = new FacetsStore(_dbContext, new DummyQueryPager());

            var func = sut.GetEnvFacetFilter(entry.Filter).Compile();

            var environment = _dbContext.Set<Environment>().ToList();

            environment.Where(func).Should().HaveCount(entry.ExpectedCount);
        }

        [Theory]
        [MemberData(nameof(GetEstablishmentFacetTestData))]
        public void ShouldReturnFilteredEstablishment(AdvancedEstablishmentFacetFilterEmptyResultTestEntry entry)
        {
            var sut = new FacetsStore(_dbContext, new DummyQueryPager());

            var func = sut.GetEstablishmentFacetFilter(entry.Filter).Compile();

            var establishment = _dbContext.Set<Establishment>().ToList();

            establishment.Where(func).Should().HaveCount(entry.ExpectedCount);
        }

        public static IEnumerable<object[]> GetEnvironmentFacetTestData()
        {
            // Equals
            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Integer, 2, ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Decimal, 3.14,ComparisonOperators.Equals),
                ExpectedCount = 2,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.String, "toto", ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Percentage, 0.12m, ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            // Not Equals
            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Integer, 2, ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Decimal, 3.14,ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.String, "toto", ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Percentage, 0.12m, ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            //StricklyGreater
            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.DateTime, new DateTime(2022, 01,02), ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Integer, 2, ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 0,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Decimal, 0.12m,ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 3,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Percentage, 0.10m,ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 2,
            }};

            //StricklyLess
            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 0,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.DateTime, new DateTime(2022, 01,02), ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Integer, 3, ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 2,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Decimal, 2.15m,ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEnvironmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEnvironmentCriterion(FacetType.Percentage, 0.13m,ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 1,
            }};
        }

        public static IEnumerable<object[]> GetEstablishmentFacetTestData()
        {
            // Equals
            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Integer, 2, ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Decimal, 3.14,ComparisonOperators.Equals),
                ExpectedCount = 2,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.String, "toto", ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Percentage, 0.12m, ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.Equals),
                ExpectedCount = 1,
            }};

            // Not Equals
            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Integer, 2, ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Decimal, 3.14,ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.String, "toto", ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Percentage, 0.12m, ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.NotEquals),
                ExpectedCount = 1,
            }};

            //StricklyGreater
            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.DateTime, new DateTime(2022, 01,02), ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Integer, 2, ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 0,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Decimal, 0.12m,ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 3,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Percentage, 0.10m,ComparisonOperators.StrictlyGreaterThan),
                ExpectedCount = 2,
            }};

            //StricklyLess
            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.DateTime, new DateTime(2022, 01,01), ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 0,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.DateTime, new DateTime(2022, 01,02), ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Integer, 3, ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 2,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Decimal, 2.15m,ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 1,
            }};

            yield return new object[] { new AdvancedEstablishmentFacetFilterEmptyResultTestEntry()
            {
                Filter = GetEstablishmentCriterion(FacetType.Percentage, 0.13m,ComparisonOperators.StrictlyLessThan),
                ExpectedCount = 1,
            }};
        }
        public class AdvancedEnvironmentFacetFilterEmptyResultTestEntry
        {
            public EnvironmentFacetsAdvancedCriterion Filter { get; set; }
            public int ExpectedCount { get; set; }
        }
        public class AdvancedEstablishmentFacetFilterEmptyResultTestEntry
        {
            public EstablishmentFacetsAdvancedCriterion Filter { get; set; }
            public int ExpectedCount { get; set; }
        }

        private void PopulateDatabase()
        {
            var environment1 = new Environment() { Id = 1, Cluster = "cluster", Domain = "domain", ProductionHost = "host", Subdomain = "subdomain", };
            var environment2 = new Environment() { Id = 2, Cluster = "cluster", Domain = "domain", ProductionHost = "host", Subdomain = "subdomain" };
            var environment3 = new Environment() { Id = 3, Cluster = "cluster", Domain = "domain", ProductionHost = "host", Subdomain = "subdomain" };

            _dbContext.AddRange(environment1, environment2, environment3);

            var establishment1 = new Establishment { Id = 1, Environment = environment1, Name = "test1", TimeZoneId = "1" };
            var establishment2 = new Establishment { Id = 2, Environment = environment2, Name = "test1", TimeZoneId = "1" };
            var establishment3 = new Establishment { Id = 3, Environment = environment3, Name = "test1", TimeZoneId = "1" };

            _dbContext.AddRange(establishment1, establishment2, establishment3);
            _dbContext.SaveChanges();


            #region EnvironmentFacet
            var facet1 = new Facet
            {
                Type = FacetType.Integer,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet2 = new Facet
            {
                Type = FacetType.Integer,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet3 = new Facet
            {
                Type = FacetType.Decimal,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };
            var facet4 = new Facet
            {
                Type = FacetType.Decimal,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };
            var facet5 = new Facet
            {
                Type = FacetType.Decimal,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet6 = new Facet
            {
                Type = FacetType.String,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet7 = new Facet
            {
                Type = FacetType.String,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet8 = new Facet
            {
                Type = FacetType.Percentage,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet9 = new Facet
            {
                Type = FacetType.Percentage,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet10 = new Facet
            {
                Type = FacetType.DateTime,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet11 = new Facet
            {
                Type = FacetType.DateTime,
                Scope = FacetScope.Environment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            _dbContext.AddRange(facet1, facet2, facet3, facet4, facet5, facet6, facet7, facet8, facet9, facet10, facet11);
            _dbContext.SaveChanges();

            var facetValue1 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment1.Id,
                IntValue = 1,
                FacetId = facet1.Id,
            };

            var facetValue2 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment2.Id,
                IntValue = 2,
                FacetId = facet2.Id,
            };

            var facetValue3 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment2.Id,
                DecimalValue = 3.14m,
                FacetId = facet3.Id,
            };

            var facetValue4 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment1.Id,
                DecimalValue = 3.14m,
                FacetId = facet4.Id,
            };

            var facetValue5 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment3.Id,
                DecimalValue = 2m,
                FacetId = facet5.Id,
            };

            var facetValue6 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment3.Id,
                StringValue = "toto",
                FacetId = facet6.Id,
            };

            var facetValue7 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment2.Id,
                StringValue = "toto2",
                FacetId = facet7.Id,
            };

            var facetValue8 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment2.Id,
                DecimalValue = 0.12m,
                FacetId = facet8.Id,
            };

            var facetValue9 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment3.Id,
                DecimalValue = 0.13m,
                FacetId = facet9.Id,
            };

            var facetValue10 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment3.Id,
                DateTimeValue = new DateTime(2022, 01, 01),
                FacetId = facet10.Id,
            };
            var facetValue11 = new EnvironmentFacetValueDao
            {
                EnvironmentId = environment2.Id,
                DateTimeValue = new DateTime(2022, 01, 05),
                FacetId = facet11.Id,
            };

            _dbContext.AddRange(facetValue1, facetValue2, facetValue3, facetValue4, facetValue5, facetValue6, facetValue7, facetValue8, facetValue9, facetValue10, facetValue11);
            _dbContext.SaveChanges();
            #endregion

            #region EstablishmentFacet
            var facet51 = new Facet
            {
                Type = FacetType.Integer,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet52 = new Facet
            {
                Type = FacetType.Integer,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet53 = new Facet
            {
                Type = FacetType.Decimal,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };
            var facet54 = new Facet
            {
                Type = FacetType.Decimal,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };
            var facet55 = new Facet
            {
                Type = FacetType.Decimal,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet56 = new Facet
            {
                Type = FacetType.String,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet57 = new Facet
            {
                Type = FacetType.String,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet58 = new Facet
            {
                Type = FacetType.Percentage,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet59 = new Facet
            {
                Type = FacetType.Percentage,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet60 = new Facet
            {
                Type = FacetType.DateTime,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            var facet61 = new Facet
            {
                Type = FacetType.DateTime,
                Scope = FacetScope.Establishment,
                Code = "code",
                ApplicationId = "applicationId"
            };

            _dbContext.AddRange(facet51, facet52, facet53, facet54, facet55, facet56, facet57, facet58, facet59, facet60, facet61);
            _dbContext.SaveChanges();

            var facetValue51 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment1.Id,
                EnvironmentId = environment1.Id,
                IntValue = 1,
                FacetId = facet51.Id,
            };

            var facetValue52 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment2.Id,
                EnvironmentId = environment2.Id,
                IntValue = 2,
                FacetId = facet52.Id,
            };

            var facetValue53 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment2.Id,
                EnvironmentId = environment2.Id,
                DecimalValue = 3.14m,
                FacetId = facet53.Id,
            };

            var facetValue54 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment1.Id,
                EnvironmentId = environment1.Id,
                DecimalValue = 3.14m,
                FacetId = facet54.Id,
            };

            var facetValue55 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment3.Id,
                EnvironmentId = environment3.Id,
                DecimalValue = 2m,
                FacetId = facet55.Id,
            };

            var facetValue56 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment3.Id,
                EnvironmentId = environment3.Id,
                StringValue = "toto",
                FacetId = facet56.Id,
            };

            var facetValue57 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment2.Id,
                EnvironmentId = environment2.Id,
                StringValue = "toto2",
                FacetId = facet57.Id,
            };

            var facetValue58 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment2.Id,
                EnvironmentId = environment2.Id,
                DecimalValue = 0.12m,
                FacetId = facet58.Id,
            };

            var facetValue59 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment3.Id,
                EnvironmentId = environment3.Id,
                DecimalValue = 0.13m,
                FacetId = facet59.Id,
            };

            var facetValue60 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment3.Id,
                EnvironmentId = environment3.Id,
                DateTimeValue = new DateTime(2022, 01, 01),
                FacetId = facet60.Id,
            };
            var facetValue61 = new EstablishmentFacetValueDao
            {
                EstablishmentId = establishment2.Id,
                EnvironmentId = environment2.Id,
                DateTimeValue = new DateTime(2022, 01, 05),
                FacetId = facet61.Id,
            };

            _dbContext.AddRange(facetValue51, facetValue52, facetValue53, facetValue54, facetValue55, facetValue56, facetValue57, facetValue58, facetValue59, facetValue60, facetValue61);
            _dbContext.SaveChanges();
            #endregion
        }

        public static EnvironmentFacetsAdvancedCriterion GetEnvironmentCriterion<T>(FacetType facetType, T value, ComparisonOperators op)
        {
            return new EnvironmentFacetsAdvancedCriterion
            {
                Value = GetEnvironmentFacetValueExpressionAccordingToFacetType(facetType, value.ToString(), op),
                ItemsMatched = ItemsMatching.Any,
                Identifier = new FacetIdentifier { ApplicationId = "applicationId", Code = "code" }
            };
        }

        private static IEnvironmentFacetCriterion GetEnvironmentFacetValueExpressionAccordingToFacetType(FacetType facetType, string value, ComparisonOperators op) => facetType switch
        {
            FacetType.Integer => new SingleFacetIntValueComparisonCriterion { Value = int.Parse(value), Type = facetType, Operator = op },
            FacetType.DateTime => new SingleFacetDateTimeValueComparisonCriterion { Value = DateTime.Parse(value), Type = facetType, Operator = op },
            FacetType.Decimal => new SingleFacetDecimalValueComparisonCriterion { Value = decimal.Parse(value), Type = facetType, Operator = op },
            FacetType.Percentage => new SingleFacetDecimalValueComparisonCriterion { Value = decimal.Parse(value), Type = facetType, Operator = op },
            FacetType.String => new SingleFacetValueComparisonCriterion<string>() { Value = value, Type = facetType, Operator = op },
            _ => throw new BadRequestException()
        };

        public static EstablishmentFacetsAdvancedCriterion GetEstablishmentCriterion<T>(FacetType facetType, T value, ComparisonOperators op)
        {
            return new EstablishmentFacetsAdvancedCriterion
            {
                Value = GetEnvironmentFacetValueExpressionAccordingToFacetType(facetType, value.ToString(), op),
                ItemsMatched = ItemsMatching.Any,
                Identifier = new FacetIdentifier { ApplicationId = "applicationId", Code = "code" }
            };
        }
    }

    internal class DummyQueryPager : IQueryPager
    {
        public IQueryable<T> GetPagedQueryable<T>(IQueryable<T> unpagedQuery, IPageToken token) where T : class
        {
            return unpagedQuery;
        }

        public async Task<Page<T>> ToPageAsync<T>(IQueryable<T> unpagedQuery, IPageToken token) where T : class
        {
            var items = await unpagedQuery.ToListAsync();
            return new Page<T>
            {
                Items = items,
                Count = items.Count,
                Prev = null,
                Next = null,
            };
        }
    }


}
