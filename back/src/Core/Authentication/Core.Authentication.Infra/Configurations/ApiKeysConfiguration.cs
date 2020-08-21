using System;
using System.Collections.Generic;

namespace Core.Authentication.Infra.Configurations
{
    public class ApiKeysConfiguration : List<ApiKeyConfiguration>
    { }

    public class ApiKeyConfiguration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Token { get; set; }
    }
}
