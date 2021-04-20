using Instances.Domain.Demos;
using System;
using TechTalk.SpecFlow;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    public class DistributorFilter
    {
        public string Code { get; }
        public bool IsWanted { get; }
        public Func<Demo, bool> AsFunc { get; }

        private DistributorFilter(string code, bool isWanted)
        {
            Code = code;
            IsWanted = isWanted;

            if (IsWanted)
            {
                AsFunc = d => d.Distributor.Code == Code;
            }
            else
            {
                AsFunc = d => d.Distributor.Code != Code;
            }
        }

        public static DistributorFilter FromCode(string code) => new DistributorFilter(code, true);
        public static DistributorFilter NotFromCode(string code) => new DistributorFilter(code, false);
    }

    public class DistributorSelection
    {
        public DistributorSelection(string code)
        {
            Code = code;
        }

        public string Code { get; }
    }

    [Binding]
    public class DemoModifiers
    {
        [StepArgumentTransformation(@"'(regular|template)'")]
        public bool IsTemplate(string value)
        {
            return value == "template";
        }

        [StepArgumentTransformation("for distributor '(.*)'")]
        public DistributorSelection SelectDistributor(string code)
        {
            return new DistributorSelection(code);
        }

        [StepArgumentTransformation("from distributor '(.*)'")]
        public DistributorFilter FilterDistributor(string code)
        {
            return DistributorFilter.FromCode(code);
        }

        [StepArgumentTransformation("from distributor other than '(.*)'")]
        public DistributorFilter FilterOutDistributor(string code)
        {
            return DistributorFilter.NotFromCode(code);
        }
    }
}
