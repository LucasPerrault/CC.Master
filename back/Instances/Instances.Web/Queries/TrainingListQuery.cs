using Instances.Domain.Trainings.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using Tools;
using Tools.Web;

namespace Instances.Web
{
    public class TrainingListQuery
    {
        public IPageToken Page { get; set; } = null;
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true };
        public int? AuthorId { get; set; } = null;
        public TrainingListInstance Instance { get; set; } = new TrainingListInstance();
        public TrainingListEnvironment Environment { get; set; } = new TrainingListEnvironment();

        public TrainingFilter ToTrainingFilter()
        {
            return new TrainingFilter
            {
                IsActive = IsActive.ToCompareBoolean(),
                AuthorId = AuthorId,
                Subdomain = string.IsNullOrEmpty(Environment?.Subdomain)  ? CompareString.Bypass : CompareString.Equals(Environment.Subdomain),
                IsProtected = Instance?.IsProtected?.ToCompareBoolean() ?? CompareBoolean.Bypass,
            };
        }
    }

    public class TrainingListInstance
    {
        public HashSet<bool> IsProtected { get;set; } = new HashSet<bool>();
    }

    public class TrainingListEnvironment
    {
        public string Subdomain { get; set; } = null;
    }
}
