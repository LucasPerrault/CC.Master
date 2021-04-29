using System;
using System.Collections.Generic;

namespace Instances.Domain.Instances
{

    public interface ISqlScriptPicker
    {
        List<Uri> GetForDuplication(InstanceDuplication duplication);
    }
}
