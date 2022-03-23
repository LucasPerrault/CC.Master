using Billing.Contracts.Domain.Common;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Billing.Contracts.Web
{
    public class AccountingPeriodJsonConverter : JsonConverter<AccountingPeriod>
    {

        public override AccountingPeriod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return AccountingPeriod.Parse(s);
        }

        public override void Write(Utf8JsonWriter writer, AccountingPeriod value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
