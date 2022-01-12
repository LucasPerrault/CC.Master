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
        private class AccountingPeriodFormatProvider : IFormatProvider
        {
            public object GetFormat(Type? formatType) => DateFormat;
        }

        private const string DateFormat = "yyyy-MM";
        private static readonly AccountingPeriodFormatProvider formatProvider = new AccountingPeriodFormatProvider();

        public override AccountingPeriod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return DateTime.Parse(s, formatProvider);
        }

        public override void Write(Utf8JsonWriter writer, AccountingPeriod value, JsonSerializerOptions options)
            => writer.WriteStringValue(((DateTime)value).ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
