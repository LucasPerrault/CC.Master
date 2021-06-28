using Billing.Products.Domain;
using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Cmrr.Domain
{
    public class CmrrSituation
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }

        public CmrrAxis Axis { get; set; }

        public CmrrSubLine Total { get; set; }
        public List<CmrrLine> Lines { get; set; }
    }

    public class CmrrLine
    {
        public string Name { get; }
        public CmrrSubLine Total { get; }
        public Dictionary<string, CmrrSubLine> SubLines { get; } = new Dictionary<string, CmrrSubLine>();

        public CmrrLine(string name)
        {
            Name = name;
            Total = new CmrrSubLine(Name);
        }
    }

    public class CmrrSubLine
    {
        public CmrrSubLine(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public CmrrAmount TotalFrom { get; set; } = new CmrrAmount();
        public CmrrAmount TotalTo { get; set; } = new CmrrAmount();

        public CmrrAmount Upsell { get; set; } = new CmrrAmount();
        public CmrrAmount Creation { get; set; } = new CmrrAmount();
        public CmrrAmount Termination { get; set; } = new CmrrAmount();
        public CmrrAmount Expansion { get; set; } = new CmrrAmount();
        public CmrrAmount Contraction { get; set; } = new CmrrAmount();
    }

    public class CmrrAmount
    {
        public decimal Amount { get; set; }
        public int ContractCount => ContractIds.Count;
        public int ClientCount => ClientIds.Count;

        public List<CmrrAmountTopElement> Top { get; } = new List<CmrrAmountTopElement>();

        internal HashSet<int> ClientIds { get; } = new HashSet<int>();
        internal HashSet<int> ContractIds { get; } = new HashSet<int>();

        public int UserCount { get; set; }

        public void AddClient(int clientId)
        {
            ClientIds.Add(clientId);
        }

        public void AddContract(int contractId)
        {
            ContractIds.Add(contractId);
        }
    }

    public class CmrrAmountTopElement
    {
        public const int TopCount = 10;
        public Breakdown Breakdown { get; private set; }
        public decimal Amount { get; private set; }
        public decimal UserCount { get; private set; }
        public CmrrAmountTopElementContract Contract { get; private set; }
        public CmrrAmountTopElementCount StartPeriodCount { get; private set; }
        public CmrrAmountTopElementCount EndPeriodCount { get; private set; }

        private CmrrAmountTopElement()
        { }

        public static CmrrAmountTopElement FromSituation(ContractAxisSectionSituation situation, decimal amount, int userCount)
        {
            return new CmrrAmountTopElement
            {
                Breakdown = situation.Breakdown,
                Contract = CmrrAmountTopElementContract.FromContract(situation.ContractSituation.Contract),
                StartPeriodCount = CmrrAmountTopElementCount.FromCount(situation.ContractSituation.StartPeriodCount),
                EndPeriodCount = CmrrAmountTopElementCount.FromCount(situation.ContractSituation.EndPeriodCount),
                Amount = amount,
                UserCount = userCount
            };
        }
    }

    public class CmrrAmountTopElementCount
    {
        public int? Id { get; private set; }
        public int AccountingNumber { get; private set; }

        private CmrrAmountTopElementCount()
        { }

        public static CmrrAmountTopElementCount FromCount(CmrrCount count)
        {
            return new CmrrAmountTopElementCount
            {
                Id = count?.Id,
                AccountingNumber = count?.AccountingNumber ?? 0,
            };
        }
    }

    public class CmrrAmountTopElementContract
    {

        public int Id { get; private set; }
        public int ClientId { get; private set; }
        public string ClientName { get; private set; }

        private CmrrAmountTopElementContract()
        { }

        public static CmrrAmountTopElementContract FromContract(CmrrContract contract)
        {
            return new CmrrAmountTopElementContract
            {
                ClientName = contract.ClientName,
                ClientId = contract.ClientId,
                Id = contract.Id
            };
        }
    }

    public class ContractAxisSectionSituation
    {
        public Breakdown Breakdown { get; }
        public CmrrContractSituation ContractSituation { get; }

        public decimal StartPeriodAmount { get; }
        public decimal EndPeriodAmount { get; }
        public decimal PartialDiff => EndPeriodAmount - StartPeriodAmount;

        public int StartPeriodUserCount { get; }
        public int EndPeriodUserCount { get; }
        public int UserCountDiff => EndPeriodUserCount - StartPeriodUserCount;

        public ContractAxisSectionSituation(Breakdown breakdown, CmrrContractSituation contractSituation)
        {
            Breakdown = breakdown;
            ContractSituation = contractSituation;

            StartPeriodAmount = breakdown.Ratio * contractSituation.StartPeriodCount?.EuroTotal ?? 0;
            EndPeriodAmount = breakdown.Ratio * contractSituation.EndPeriodCount?.EuroTotal ?? 0;

            StartPeriodUserCount = contractSituation.StartPeriodCount?.AccountingNumber ?? 0;
            EndPeriodUserCount = contractSituation.EndPeriodCount?.AccountingNumber ?? 0;
        }
    }

    public class Breakdown : ValueObject
    {
        public AxisSection AxisSection { get; set; }
        public int ProductId { get; set; }
        public decimal Ratio { get; set; }
        public string SubSection { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return AxisSection;
                yield return Ratio;
                yield return SubSection;
                yield return ProductId;
            }
        }
    }

    public class AxisSection : ValueObject
    {
        public int Id { get; }
        public string Name { get; }
        public int Order { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Id;
                yield return Name;
                yield return Order;
            }
        }

        private AxisSection(int id, string name, int order)
        {
            Id = id;
            Name = name;
            Order = order;
        }

        public static AxisSection ForBusinessUnit(BusinessUnit bu) => new AxisSection(bu.Id, bu.Name, bu.DisplayOrder);
        public static AxisSection ForProductFamily(ProductFamily family) => new AxisSection(family.Id, family.Name, family.DisplayOrder);
    }
}
