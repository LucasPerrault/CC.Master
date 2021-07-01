using System;
using System.Collections.Generic;

namespace Billing.Cmrr.Domain.Situation
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

        private HashSet<int> ClientIds { get; } = new HashSet<int>();
        private HashSet<int> ContractIds { get; } = new HashSet<int>();

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
        public int Id { get; private set; }
        public int AccountingNumber { get; private set; }

        private CmrrAmountTopElementCount()
        { }

        public static CmrrAmountTopElementCount FromCount(CmrrCount count)
        {
            if (count is null)
            {
                return null;
            }

            return new CmrrAmountTopElementCount
            {
                Id = count.Id,
                AccountingNumber = count.AccountingNumber,
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
}
