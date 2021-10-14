using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedFilters.Domain.Core.Models
{
    public interface IDeepCopyable<T>
    {
        T DeepCopy();
    }
}
