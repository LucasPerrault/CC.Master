using FluentAssertions;
using System;
using Xunit;

namespace Billing.Cmrr.Domain.Tests
{
    public class CmrrContratSituationTests
    {
        [Fact]
        public void ShouldReturnContractSituationHasUpsell()
        {
            var contractSituation = new CmrrContratSituation
            {
                Contract = new CmrrContract
                {
                    CreationCause = ContractCreationCause.Modification,
                    EnvironmentCreatedAt = new DateTime(2021, 01, 01),
                    StartDate = new DateTime(2022, 01, 01)
                },
                EndPeriodCount = new CmrrCount(),
                StartPeriodCount = null
            };

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Upsell);
        }

        [Fact]
        public void ShouldReturnContractSituationHasCreation()
        {
            var contractSitation = new CmrrContratSituation
            {
                Contract = new CmrrContract
                {
                    CreationCause = ContractCreationCause.NewBooking,
                    EnvironmentCreatedAt = new DateTime(2021, 01, 01),
                    StartDate = new DateTime(2021, 01, 01)
                },
                EndPeriodCount = new CmrrCount(),
                StartPeriodCount = null
            };

            contractSitation.LifeCycle.Should().Be(CmrrLifeCycle.Creation);
        }

        [Fact]
        public void ShouldReturnContractSituationHasTermination()
        {
            var contractSituation = new CmrrContratSituation
            {
                Contract = new CmrrContract
                {
                    EndReason = ContractEndReason.Resiliation,
                },
                StartPeriodCount = new CmrrCount(),
                EndPeriodCount = null
            };

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Termination);
        }

        [Fact]
        public void ShouldReturnContractSituationHasRetractionWithNoEndPeriodCount()
        {
            var contractSituation = new CmrrContratSituation
            {
                Contract = new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                StartPeriodCount = new CmrrCount(),
                EndPeriodCount = null
            };

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Retraction);
        }

        [Fact]
        public void ShouldReturnContractSituationHasRetractionWithEndPeriodCount()
        {
            var contractSituation = new CmrrContratSituation
            {
                Contract = new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                StartPeriodCount = new CmrrCount { EuroTotal = 1500 },
                EndPeriodCount = new CmrrCount { EuroTotal = 1000 }
            };

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Retraction);
        }

        [Fact]
        public void ShouldReturnContractSituationHasExpansion()
        {
            var contractSituation = new CmrrContratSituation
            {
                Contract = new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                StartPeriodCount = new CmrrCount { EuroTotal = 1000 },
                EndPeriodCount = new CmrrCount { EuroTotal = 1500 }
            };

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Expansion);
        }
    }
}
