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
            var contractSituation = new CmrrContractSituation(
            
                new CmrrContract
                {
                    CreationCause = ContractCreationCause.NewBooking,
                    EnvironmentCreatedAt = new DateTime(2021, 01, 01),
                    StartDate = new DateTime(2022, 01, 01)
                },
                null,
                new CmrrCount()
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Upsell);
        }

        [Fact]
        public void ShouldReturnContractSituationHasCreation()
        {
            var contractSitation = new CmrrContractSituation(

                new CmrrContract
                {
                    CreationCause = ContractCreationCause.NewBooking,
                    EnvironmentCreatedAt = new DateTime(2021, 01, 01),
                    StartDate = new DateTime(2021, 01, 01)
                },
                null,
                new CmrrCount()
            );

            contractSitation.LifeCycle.Should().Be(CmrrLifeCycle.Creation);
        }

        [Fact]
        public void ShouldReturnContractSituationHasCreationWithContractModification()
        {
            var contractSitation = new CmrrContractSituation(

                new CmrrContract
                {
                    CreationCause = ContractCreationCause.Modification,
                    EnvironmentCreatedAt = new DateTime(2021, 01, 01),
                    StartDate = new DateTime(2022, 01, 01)
                },
                null,
                new CmrrCount()
            );

            contractSitation.LifeCycle.Should().Be(CmrrLifeCycle.Creation);
        }

        [Fact]
        public void ShouldReturnContractSituationHasTermination()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    EndReason = ContractEndReason.Resiliation,
                },
                new CmrrCount(),
                null
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Termination);
        }

        [Fact]
        public void ShouldReturnContractSituationHasRetractionWithNoEndPeriodCount()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                new CmrrCount(),
                null
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Retraction);
        }

        [Fact]
        public void ShouldReturnContractSituationHasRetractionWithEndPeriodCount()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                new CmrrCount { EuroTotal = 1500 },
                new CmrrCount { EuroTotal = 1000 }
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Retraction);
        }

        [Fact]
        public void ShouldReturnContractSituationHasExpansion()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                new CmrrCount { EuroTotal = 1000 },
                new CmrrCount { EuroTotal = 1500 }
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Expansion);
        }
    }
}
