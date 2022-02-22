using AdvancedFilters.Domain.Facets;
using System;
using AdvancedFilters.Domain.Instance.Models;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Storage.DAO
{
    public class EstablishmentFacetValueDao : IFacetValueDao
    {
        public int Id { get; set; }
        public int FacetId { get; set; }
        public int EnvironmentId { get; set; }
        public int EstablishmentId { get; set; }

        public int? IntValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public string StringValue { get; set; }

        public Facet Facet { get; set; }
        public Environment Environment { get; set; }
        public Establishment Establishment { get; set; }
    }
}
