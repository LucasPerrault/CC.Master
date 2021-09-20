using Lucca.Core.PublicData.Domain.Models;
using System;
using System.Collections.Generic;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class LegalUnit
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string LegalIdentificationNumber { get; set; }
        public string ActivityCode { get; set; }
        public int CountryId { get; set; }
        public int HeadquartersId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }

        public Environment Environment { get; set; }
        public IReadOnlyCollection<Establishment> Establishments { get; set; }

        public Country Country { get; set; }
    }

    public class Country
    {
        public int Id { get; }
        public string Name { get; }

        public Country(LuccaCountry luccaCountry)
        {
            Id = luccaCountry.Id;
            Name = luccaCountry.Name;
        }
    }
}
