using Distributors.Domain.Models;
using System;

namespace Environments.Domain
{

    //  any change on this enum should be made identically to joining table
    //  /SGBD/Updaters/Views/EnvironmentSharedAccesses.sql
    public enum EnvironmentAccessTypeEnum
    {
        Manual = 0,
        EnvironmentCreation = 1,
        Contract = 2
    }

    public enum EnvironmentAccessLifecycleStep
    {
        NotStarted = 0,
        InProgress = 1,
        Revoked = 2,
        Ended = 3
    }

    public class EnvironmentSharedAccess
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public int ConsumerId { get; set; }

        public EnvironmentAccess Access { get; set; }
        public Distributor Consumer { get; set; }
    }

    public class EnvironmentAccess
    {
        public int Id { get; set; }
        public int DistributorId { get; set; }
        public int EnvironmentId { get; set; }
        public int AuthorId { get; set; }
        public EnvironmentAccessTypeEnum Type { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public string Comment { get; set; }
        public string RevocationComment { get; set; }

        public EnvironmentAccessLifecycleStep Lifecycle { get; private set; }
    }
}
