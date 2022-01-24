using AdvancedFilters.Domain.Facets;
using System;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Storage.DAO
{
    public class EnvironmentFacetValueDao
    {
        public int Id { get; set; }
        public int FacetId { get; set; }
        public int EnvironmentId { get; set; }

        public int? IntValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public string StringValue { get; set; }

        public Facet Facet { get; set; }
        public Environment Environment { get; set; }
    }
}
