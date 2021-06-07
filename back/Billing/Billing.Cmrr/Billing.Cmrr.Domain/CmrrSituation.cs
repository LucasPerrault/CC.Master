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
        public List<CmrrLine> Lines { get; set; }
    }

    public class CmrrLine
    {
        public CmrrAxisSection Section { get; }
        public Dictionary<string, CmrrAxisSection> SubSections { get; } = new Dictionary<string, CmrrAxisSection>();

        public CmrrLine(string name)
        {
            Section = new CmrrAxisSection(name);
        }
    }

    public class CmrrAxisSection
    {
        public CmrrAxisSection(string name)
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
        public CmrrAmount Retraction { get; set; } = new CmrrAmount();
    }

    public class CmrrAmount
    {
        public decimal Amount { get; set; }
        public List<CmrrAmountTopElement> Top { get; } = new List<CmrrAmountTopElement>();
    }

    public class CmrrAmountTopElement
    {
        public const int TopCount = 10;
        public Breakdown Breakdown { get; private set; }
        public CmrrAmountTopElementContract Contract { get; private set; }
        public CmrrAmountTopElementCount StartPeriodCount { get; private set; }
        public CmrrAmountTopElementCount EndPeriodCount { get; private set; }

        private CmrrAmountTopElement()
        { }

        public static CmrrAmountTopElement FromSituation(ContractAxisSectionSituation situation)
        {
            return new CmrrAmountTopElement
            {
                Breakdown = situation.Breakdown,
                Contract = CmrrAmountTopElementContract.FromContract(situation.ContractSituation.Contract),
                StartPeriodCount = CmrrAmountTopElementCount.FromCount(situation.ContractSituation.StartPeriodCount),
                EndPeriodCount = CmrrAmountTopElementCount.FromCount(situation.ContractSituation.EndPeriodCount),
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

        public ContractAxisSectionSituation(Breakdown breakdown, CmrrContractSituation contractSituation)
        {
            Breakdown = breakdown;
            ContractSituation = contractSituation;

            StartPeriodAmount = breakdown.Ratio * contractSituation.StartPeriodCount?.EuroTotal ?? 0;
            EndPeriodAmount = breakdown.Ratio * contractSituation.EndPeriodCount?.EuroTotal ?? 0;
        }
    }

    public class Breakdown
    {
        public AxisSection AxisSection { get; set; }
        public decimal Ratio { get; set; }
        public string SubSection { get; set; }
    }

    public class AxisSection : ValueObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Id;
                yield return Name;
            }
            set
            {
                EqualityComponents = value;
            }
        }
    }
}
