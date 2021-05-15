using Instances.Application.Specflow.Tests.Demos.Models;
using TechTalk.SpecFlow;
using Xunit;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    [Binding]
    public class ExceptionStep
    {
        private readonly DemosContext _demosContext;
        public ExceptionStep(DemosContext demosContext)
        {
            _demosContext = demosContext;
        }

        [Then(@"user should get error containing '(.*)'")]
        public void ThenUserShouldGetErrorContainingAsync(string errorMessageExtract)
        {
            Assert.Contains(errorMessageExtract, _demosContext.Results.Exception.Message);
        }

        [Then(@"user should not get error")]
        public void ThenUserShouldNotGetError()
        {
            Assert.Null(_demosContext.Results.Exception);
        }
    }
}
