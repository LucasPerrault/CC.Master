using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Counts
{
    public class Count
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public AccountingPeriod CountPeriod { get; set; }
        public int CommercialOfferId { get; set; }
        public int Number { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal FixedPrice { get; set; }
        public bool IsMinimalBilling { get; set; }
        public decimal TotalInCurrency => FixedPrice + UnitPrice * Number;
        public Contract Contract { get; set; }
        public List<CountDetail> Details { get; set; }
    }

    public class CountDetail
    {
        public int Id { get; set; }
        public int CountId { get; set; }
        public int Number { get; set; }
        public int EstablishmentId { get; set; }
    }

    public class EnvironmentWithContractGroup
    {
        public int EnvironmentGroupId { get; set; }
        public List<Contract> Contracts { get; set; }
    }

    public class ContractWithCountNumber
    {
        public Contract Contract { get; set; }
        public AccountingPeriod Period { get; set; }
        public int CountNumber { get; set; }
        public int CountNumberWithFreeMonths { get; set; }
        public List<CountDetail> Details { get; set; }
    }
}
