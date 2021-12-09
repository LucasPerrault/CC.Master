#nullable enable
using Billing.Contracts.Domain.Common;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Billing.Contracts.Web
{
    public class AccountingPeriodJsonConverter : JsonConverter<AccountingPeriod>
    {
        private const string DateFormat = "yyyy-MM";

        public override AccountingPeriod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetDateTime();

        public override void Write(Utf8JsonWriter writer, AccountingPeriod value, JsonSerializerOptions options)
            => writer.WriteStringValue(((DateTime)value).ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
