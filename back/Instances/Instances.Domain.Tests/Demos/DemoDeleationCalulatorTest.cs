using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using System;
using Xunit;

namespace Instances.Domain.Tests.Demos
{
    public class DemoDeletionCalculatorTest
    {

        [Fact]
        public void ShouldKnowWhenStandardDemoShouldBeDeleted()
        {
            var calculator = new DemoDeletionCalculator();
            var deletionDate = calculator.GetDeletionDate(new Demo { AuthorId = 123 }, new DateTime(2010, 01, 01));
            Assert.Equal(deletionDate, new DateTime(2010, 03, 04));
        }

        [Fact]
        public void ShouldKnowWhenHubspotDemoShouldBeDeleted()
        {
            var calculator = new DemoDeletionCalculator();
            var deletionDate = calculator.GetDeletionDate(new Demo { AuthorId = 0 }, new DateTime(2010, 01, 01));
            Assert.Equal(deletionDate, new DateTime(2010, 02, 1));
        }
    }
}
