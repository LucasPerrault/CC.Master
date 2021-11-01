using Instances.Domain.Instances.Models;
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

    }
}
