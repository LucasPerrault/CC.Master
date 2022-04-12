using Environments.Domain;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Instances.Domain.Trainings;
using Instances.Infra.Storage;
using System.Collections.Generic;
using Testing.Infra;
using Testing.Specflow;

namespace Instances.Application.Specflow.Tests.Trainings.Models
{
    public class TrainingRestorationEndTestResults
    {
        public TrainingRestoration GivenRestoration { get; set; }
        public Instance CreatedInstance { get; set; }
    }
}
