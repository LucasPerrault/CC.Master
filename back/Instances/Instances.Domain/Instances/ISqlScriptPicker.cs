using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{

    public interface ISqlScriptPicker
    {
        Task<List<Uri>> GetForAnonymizationAsync(InstanceDuplication duplication);
        Task<List<Uri>> GetForCleaningAsync(InstanceDuplication duplication);
        Task<List<Uri>> GetExtraForDuplicationAsync(InstanceDuplication duplication);
        Task<List<Uri>> GetPreRestoreScriptsAsync(InstanceDuplication duplication, InstanceDuplicationOptions duplicationOptions);
        Task<List<Uri>> GetPostRestoreScriptsAsync(InstanceDuplication duplication, InstanceDuplicationOptions duplicationOptions);

    }
}
