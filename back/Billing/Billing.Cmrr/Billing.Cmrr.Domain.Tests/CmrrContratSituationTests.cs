using FluentAssertions;
using System;
using Xunit;

namespace Billing.Cmrr.Domain.Tests
{
    public class CmrrContratSituationTests
    {
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
        public void ShouldReturnContractSituationHasExpansion()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    CreationCause = ContractCreationCause.NewBooking,
                    StartDate = new DateTime(2022, 01, 01)
                },
                new CmrrCount { EuroTotal = 1000 },
                new CmrrCount { EuroTotal = 1500 }
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Expansion);
        }


        [Fact]
        public void ShouldReturnContractSituationHasExpansion2()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    CreationCause = ContractCreationCause.Modification,
                    StartDate = new DateTime(2022, 01, 01)
                },
                new CmrrCount { EuroTotal = 1000 },
                new CmrrCount { EuroTotal = 1500 }
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Expansion);
        }

        [Fact]
        public void ShouldReturnContractSituationHasExpansionWithContractModification()
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

            contractSitation.LifeCycle.Should().Be(CmrrLifeCycle.Expansion);
        }

        [Fact]
        public void ShouldReturnContractSituationHasContractionWithAmount()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    EndReason = ContractEndReason.Resiliation,
                },
                new CmrrCount { EuroTotal = 1500 },
                new CmrrCount { EuroTotal = 1000 }
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Contraction);
        }

        [Fact]
        public void ShouldReturnContractSituationHasContractionWithEndPeriodCount()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract
                {
                    EndReason = ContractEndReason.Modification,
                },
                new CmrrCount(),
                new CmrrCount()
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Contraction);
        }

        [Fact]
        public void ShouldReturnContractSituationHasContractionWithNoEndPeriodCount()
        {
            var contractSituation = new CmrrContractSituation(
                new CmrrContract(),
                new CmrrCount(),
                null
            );

            contractSituation.LifeCycle.Should().Be(CmrrLifeCycle.Contraction);
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

    }
}
