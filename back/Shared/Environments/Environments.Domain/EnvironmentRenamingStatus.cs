using System.Collections.Generic;
using System.Linq;

namespace Environments.Domain
{
    public enum EnvironmentRenamingStatus
    {
        SUCCESS,
        ERROR
    }

    public record EnvironmentRenamingStatusDetail(IEnumerable<string> ErrorMessages)
    {
        public EnvironmentRenamingStatus Status => ErrorMessages.Any() ?
            EnvironmentRenamingStatus.ERROR :
            EnvironmentRenamingStatus.SUCCESS;
    }

}

