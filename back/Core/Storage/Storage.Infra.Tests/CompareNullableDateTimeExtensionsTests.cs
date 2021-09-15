using Storage.Infra.Extensions;
using Storage.Infra.Tests.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using Xunit;

namespace Storage.Infra.Tests
{
    public class CompareNullableDateTimeExtensionsTests
    {

        [Theory]
        [MemberData(nameof(TestElementsAsObjects))]
        public void ShouldProperlyFilterDates(DateTime?[] ok, DateTime?[] ko, CompareNullableDateTime comparer)
        {
            ComparerTestsHelper.ShouldProperlyFilter(ok, ko, dates => dates.Apply(comparer).To(d => d));
        }

        public static IEnumerable<object[]> TestElementsAsObjects() => TestElements().Select(e => e.ToObjects());

        private static IEnumerable<TestElement<CompareNullableDateTime, DateTime?>> TestElements()
        {
            yield return TestElement<CompareNullableDateTime, DateTime?>
                .ForComparer(CompareNullableDateTime.AnyNotNull())
                .Accepts(new DateTime(2021, 01, 01))
                .AndRejects((DateTime?)null);

            yield return TestElement<CompareNullableDateTime, DateTime?>
                .ForComparer(CompareNullableDateTime.IsNull())
                .Accepts((DateTime?)null)
                .AndRejects(new DateTime(2021, 01, 01));

            yield return TestElement<CompareNullableDateTime, DateTime?>
                .ForComparer(CompareDateTime.IsStrictlyBefore(new DateTime(2021, 01, 02)).OrNull())
                .Accepts(new DateTime(2021, 01, 01), null)
                .AndRejects(new DateTime(2021, 01, 02));

            yield return TestElement<CompareNullableDateTime, DateTime?>
                .ForComparer(CompareDateTime.IsStrictlyBefore(new DateTime(2021, 01, 02)).AndNotNull())
                .Accepts(new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2021, 01, 02), null);
        }
    }
}
