﻿using Distributors.Domain.Models;

namespace Instances.Infra.DataDuplication
{
    public enum DatabaseType
    {
        Production = 1,
        Training = 2,
        Preview = 3,
        Demos = 4
    }

    public class TenantDataDuplication
    {
        public DatabaseType Type { get; set; }
        public Distributor Distributor { get; set; }
        public string TargetCluster { get; set; }
        public string SourceDemoCluster { get; set; }
    }
}
