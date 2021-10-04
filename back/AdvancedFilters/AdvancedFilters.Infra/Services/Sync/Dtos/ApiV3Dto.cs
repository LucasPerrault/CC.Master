using System.Collections.Generic;

namespace AdvancedFilters.Infra.Services.Sync.Dtos
{
    internal class ApiV3Dto<T>
        where T : class
    {
        public ApiV3DtoData<T> Data { get; set; }
    }

    internal class ApiV3DtoData<T>
    {
        public List<T> Items { get; set; }
    }
}
