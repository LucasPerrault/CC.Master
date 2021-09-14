using FluentAssertions;
using Storage.Infra.Extensions;
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
            var allDates = ok.ToList();
            allDates.AddRange(ko);
            var filtered = allDates.AsQueryable().Apply(comparer).To(date => date).ToList();
            filtered.Count.Should().Be(ok.Length);
            filtered.Should().BeEquivalentTo(ok);
        }

        public static IEnumerable<object[]> TestElementsAsObjects() => TestElements().Select(e => e.ToObjects());

        private static IEnumerable<TestElement> TestElements()
        {
            yield return TestElement
                .ForComparer(CompareDateTime.IsStrictlyAfter(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2021, 01, 02))
                .AndRejects(new DateTime(2021, 01, 01), new DateTime(2020, 12, 31));

            yield return TestElement
                .ForComparer(CompareDateTime.IsAfterOrEqual(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2021, 01, 02), new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2020, 12, 31));

            yield return TestElement
                .ForComparer(CompareDateTime.IsBetweenOrEqual(new DateTime(2021, 01, 01), new DateTime(2021, 01, 02)))
                .Accepts(new DateTime(2021, 01, 02), new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03));

            yield return TestElement
                .ForComparer(CompareDateTime.IsStrictlyBetween(new DateTime(2021, 01, 01), new DateTime(2021, 01, 02)))
                .Accepts()
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03), new DateTime(2021, 01, 02), new DateTime(2021, 01, 01));

            yield return TestElement
                .ForComparer(CompareDateTime.IsStrictlyBetween(new DateTime(2021, 01, 01), new DateTime(2021, 01, 03)))
                .Accepts(new DateTime(2021, 01, 02))
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03), new DateTime(2021, 01, 01));

            yield return TestElement
                .ForComparer(CompareDateTime.IsBeforeOrEqual(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2020, 12, 31),  new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2021, 01, 03), new DateTime(2021, 01, 02));

            yield return TestElement
                .ForComparer(CompareDateTime.IsStrictlyBefore(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2020, 12, 31))
                .AndRejects(new DateTime(2021, 01, 01), new DateTime(2021, 01, 03), new DateTime(2021, 01, 02));

            yield return TestElement
                .ForComparer(CompareDateTime.IsEqual(new DateTime(2021, 01, 01)))
                .Accepts(new DateTime(2021, 01, 01))
                .AndRejects(new DateTime(2020, 12, 31), new DateTime(2021, 01, 03), new DateTime(2021, 01, 02));
        }

        internal class TestElement
        {
            private CompareDateTime CompareDateTime { get; set; }
            private DateTime[] Accepted { get; set; }
            private DateTime[] Rejected { get; set; }

            private TestElement() { }

            public object[] ToObjects() => new object[]{ Accepted, Rejected, CompareDateTime };

            public static IIsOk ForComparer(CompareDateTime compareDateTime) => new TestDataBuilder(compareDateTime);

            internal interface IIsOk
            {
                IIsKo Accepts(params DateTime[] dateTimes);
            }

            internal interface IIsKo
            {
                TestElement AndRejects(params DateTime[] dateTimes);
            }

            private class TestDataBuilder : IIsKo, IIsOk
            {
                private readonly TestElement _testElement;

                public TestDataBuilder(CompareDateTime compareDateTime)
                {
                    _testElement = new TestElement { CompareDateTime = compareDateTime };
                }
                public TestElement AndRejects(DateTime[] dateTimes)
                {
                    _testElement.Rejected = dateTimes;
                    return _testElement;
                }

                public IIsKo Accepts(DateTime[] dateTimes)
                {
                    _testElement.Accepted = dateTimes;
                    return this;
                }
            }
        }
    }
}
