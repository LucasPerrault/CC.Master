using Environments.Domain;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Environments.Web
{
    public class DomainEnumJsonConverter : JsonConverter<EnvironmentDomain>
    {
        public override EnvironmentDomain Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var description = reader.GetString();
            var value = description.ToEnvironmentDomain();
            if (value.HasValue)
            {
                return value.Value;
            }

            throw new BadRequestException($"Unknown environment domain {description}");
        }

        public override void Write(Utf8JsonWriter writer,
            EnvironmentDomain value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToDescription());
        }
    }
}
