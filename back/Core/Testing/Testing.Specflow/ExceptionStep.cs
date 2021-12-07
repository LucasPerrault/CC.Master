﻿using TechTalk.SpecFlow;
using Xunit;

namespace Testing.Specflow
{

    [Binding]
    public class ExceptionStep
    {
        private readonly SpecflowTestContext _testContext;
        public ExceptionStep(SpecflowTestContext testContext)
        {
            _testContext = testContext;
        }

        [Then(@"user should get an error containing '(.*)'")]
        public void ThenUserShouldGetErrorContaining(string errorMessageExtract)
        {
            Assert.NotNull(_testContext.ThrownException);
            Assert.Contains(errorMessageExtract, _testContext.ThrownException.Message);
        }

        [Then(@"user should not get an error")]
        public void ThenUserShouldNotGetError()
        {
            Assert.Null(_testContext.ThrownException);
        }
    }
}
