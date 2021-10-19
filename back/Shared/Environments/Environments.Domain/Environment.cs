using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Tools;

namespace Environments.Domain
{
    public class Environment
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public EnvironmentDomain Domain { get; set; }
        public EnvironmentPurpose Purpose { get; set; }
        public string Cluster { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ProductionHost => $"https://{Subdomain}.{Domain.GetDescription()}";

        [JsonIgnore]
        public ICollection<EnvironmentSharedAccess> ActiveAccesses { get; set; }
    }

    public enum EnvironmentDomain
    {
        [Description("ilucca.net")]
        ILuccaDotNet = 0,

        [Description("ilucca.ch")]
        ILuccaDotCh = 2,

        [Description("dauphine.fr")]
        DauphineDotFr = 5
    }

    public enum EnvironmentPurpose
    {
        Contractual = 0,
        Lucca = 1,
        InternalUse = 2,
        QA = 3,
        Virgin = 4,
        Cluster = 5,
        Security = 6,
        InternalTest = 7,
        ExternalTest = 8,
        UrbaHack = 9
    }
}
