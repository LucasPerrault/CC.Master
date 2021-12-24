using System;

namespace Environments.Domain
{
    public class EnvironmentRenaming
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string ApiKeyStorageId { get; set; }
        public DateTime RenamedOn { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
        public EnvironmentRenamingStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public int EnvironmentId { get; set; }
        public Environment Environment { get; set; }
    }
}

