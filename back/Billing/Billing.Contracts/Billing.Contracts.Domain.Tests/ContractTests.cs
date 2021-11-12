using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Environments;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Billing.Contracts.Domain.Tests
{
    public class ContractTests
    {
        [Fact]
        public void EndsOnShouldBehaveProperly()
        {
            var contract = new Contract
            {
                Attachments = new List<EstablishmentAttachment>()
            };
            contract.EndsOn.Should().BeNull();

            contract.TheoreticalEndOn = new DateTime(2000, 01, 01);
            contract.EndsOn.Should().Be(new DateTime(2000, 01, 01));

            contract.Attachments.Add(new EstablishmentAttachment { EndsOn = new DateTime(1900, 01, 01)});
            contract.EndsOn.Should().Be(new DateTime(1900, 01, 01));

            contract.Attachments.Add(new EstablishmentAttachment());
            contract.EndsOn.Should().Be(new DateTime(2000, 01, 01));

            contract.TheoreticalEndOn = null;
            contract.EndsOn.Should().BeNull();

            contract.Attachments.Single(a => !a.EndsOn.HasValue).EndsOn = new DateTime(1990, 01, 01);
            contract.EndsOn.Should().Be(new DateTime(1990, 01, 01));

            contract.TheoreticalEndOn = new DateTime(9999, 01, 01);
            contract.EndsOn.Should().Be(new DateTime(1990, 01, 01));
        }
    }
}
