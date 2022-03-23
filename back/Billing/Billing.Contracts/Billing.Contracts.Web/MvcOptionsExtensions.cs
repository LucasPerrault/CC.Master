using System;
using System.ComponentModel;
using Billing.Contracts.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Tools.Web;

namespace Billing.Contracts.Web;

public static class MvcOptionsExtensions
{
    public static MvcOptions AddAccountingPeriodTypeConverter(this MvcOptions options)
    {
        TypeDescriptor.AddAttributes(typeof(AccountingPeriod), new TypeConverterAttribute(typeof(AccountingPeriodTypeConverter)));
        return options;
    }

    internal class AccountingPeriodTypeConverter : StringTypeConverter<AccountingPeriod>
    {
        protected override AccountingPeriod Parse(string s) => DateTime.Parse(s);

        protected override string ToIsoString(AccountingPeriod source) => source.ToString();
    }
}
