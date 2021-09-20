using Storage.Infra.Extensions;
using Storage.Infra.Tests.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using Xunit;

namespace Storage.Infra.Tests
{

    public class CompareDateTimeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(TestElementsAsObjects))]
        public void ShouldProperlyFilterDates(DateTime[] ok, DateTime[] ko, CompareDateTime comparer)
        {
            ComparerTestsHelper.ShouldProperlyFilter(ok, ko, dates => dates.Apply(comparer).To(d => d));
        }

        public static IEnumerable<object[]> TestElementsAsObjects() => TestElements().Select(e => e.ToObjects());

        private static IEnumerable<TestElement<CompareDateTime, DateTime>> TestElements()
        {
            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsStrictlyAfter(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2021, 01, 02))
                .AndRejects(new DateTime(2021, 01, 01), new DateTime(2020, 12, 31));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsAfterOrEqual(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2021, 01, 02), new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2020, 12, 31));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsBetweenOrEqual(new DateTime(2021, 01, 01), new DateTime(2021, 01, 02)))
                .Accepts(new DateTime(2021, 01, 02), new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsStrictlyBetween(new DateTime(2021, 01, 01), new DateTime(2021, 01, 02)))
                .Accepts()
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03), new DateTime(2021, 01, 02), new DateTime(2021, 01, 01));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsStrictlyBetween(new DateTime(2021, 01, 01), new DateTime(2021, 01, 03)))
                .Accepts(new DateTime(2021, 01, 02))
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03), new DateTime(2021, 01, 01));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsBeforeOrEqual(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2020, 12, 31),  new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2021, 01, 03), new DateTime(2021, 01, 02));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsStrictlyBefore(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2020, 12, 31))
                .AndRejects(new DateTime(2021, 01, 01), new DateTime(2021, 01, 03), new DateTime(2021, 01, 02));

            yield return TestElement<CompareDateTime, DateTime>
                .ForComparer(CompareDateTime.IsEqual(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03), new DateTime(2021, 01, 02));
        }
    }
}
