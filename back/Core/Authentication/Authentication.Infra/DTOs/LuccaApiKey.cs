using System;

namespace Authentication.Infra.DTOs
{
    public class LuccaApiKey
    {

        public static readonly string ApiFields = $"{nameof(Token)},{nameof(Name)}";

        public Guid Token { get; set; }
        public string Name { get; set; }
    }
}
