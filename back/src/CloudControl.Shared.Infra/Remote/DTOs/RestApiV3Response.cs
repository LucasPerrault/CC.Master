using System.Collections.Generic;

namespace CloudControl.Shared.Infra.Remote.DTOs
{
    public class RestApiV3Error
    {
        public string Message { get; set; }
    }

    public class RestApiV3Response<T>
    {
        public T Data { get; set; }
    }

    public class RestApiV3CollectionResponse<T> : RestApiV3Response<RestApiV3Collection<T>>
    { }

    public class RestApiV3Collection<T>
    {
        public int? Count { get; set; }
        public ICollection<T> Items { get; set; }
    }
}
