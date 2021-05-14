using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using System;

namespace Instances.Application.Demos.Duplication
{
    public static class DuplicationFactory
    {
        public static DemoDuplication New
        (
            string distributorId,
            int authorId,
            Demo sourceDemo,
            string targetCluster,
            string targetSubdomain,
            string password,
            string comment = ""
        )
        {
            var instanceDuplication = new InstanceDuplication
            {
                Id = Guid.NewGuid(),
                SourceType = InstanceType.Demo,
                TargetType = InstanceType.Demo,
                DistributorId = distributorId,
                SourceCluster = sourceDemo.Instance.Cluster,
                TargetCluster = targetCluster,
                SourceSubdomain = sourceDemo.Subdomain,
                TargetSubdomain = targetSubdomain,
                Progress = InstanceDuplicationProgress.Pending
            };

            return new DemoDuplication
            {
                InstanceDuplication = instanceDuplication,
                SourceDemoId = sourceDemo.Id,
                CreatedAt = DateTime.Now,
                AuthorId = authorId,
                Password = password,
                Comment = comment
            };
        }
    }
}
