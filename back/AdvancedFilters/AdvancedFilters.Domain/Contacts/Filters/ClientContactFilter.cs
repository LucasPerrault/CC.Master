using System;
using System.Collections.Generic;
using Tools;

namespace AdvancedFilters.Domain.Contacts.Filters
{
    public class ClientContactFilter
    {
        public IReadOnlyCollection<string> RoleCodes { get; set; }
        public IReadOnlyCollection<Guid> ClientIds { get; set; }
        public IReadOnlyCollection<int> EnvironmentIds { get; set; }
        public IReadOnlyCollection<int> EstablishmentIds { get; set; }
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsConfirmed { get; set; } = CompareBoolean.Bypass;
    }
}
