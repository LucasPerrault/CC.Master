﻿using System;
using TechTalk.SpecFlow;
using Xunit;

namespace Testing.Specflow
{

    public class ExceptionResult
    {
        public Exception Exception { get; set; }
    }

    [Binding]
    public class ExceptionStep
    {
        private readonly ExceptionResult _exceptionResult;
        public ExceptionStep(ExceptionResult exceptionResult)
        {
            _exceptionResult = exceptionResult;
        }

        [Then(@"user should get an error containing '(.*)'")]
        public void ThenUserShouldGetErrorContaining(string errorMessageExtract)
        {
            Assert.NotNull(_exceptionResult.Exception);
            Assert.Contains(errorMessageExtract, _exceptionResult.Exception.Message);
        }

        [Then(@"user should not get an error")]
        public void ThenUserShouldNotGetError()
        {
            Assert.Null(_exceptionResult.Exception);
        }
    }
}
