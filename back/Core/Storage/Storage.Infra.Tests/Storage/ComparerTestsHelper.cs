using FluentAssertions;
using System;
using System.Linq;

namespace Storage.Infra.Tests.Storage
{
    public class ComparerTestsHelper
    {
        public static void ShouldProperlyFilter<TCompared>(TCompared[] ok, TCompared[] ko, Func<IQueryable<TCompared>, IQueryable<TCompared>> filterFunc)
        {
            filterFunc(ok.AsQueryable()).Should().BeEquivalentTo(ok);
            filterFunc(ko.AsQueryable()).Should().BeEmpty();
        }
    }
}
