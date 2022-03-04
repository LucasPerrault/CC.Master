using AdvancedFilters.Domain.Facets.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class EstablishmentFacetValueDaosConfiguration : IEntityTypeConfiguration<EstablishmentFacetValueDao>
    {
        public void Configure(EntityTypeBuilder<EstablishmentFacetValueDao> builder)
        {
            builder.ToTable("EstablishmentFacetValues");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.FacetId).IsRequired();
            builder.Property(f => f.EnvironmentId).IsRequired();
            builder.Property(f => f.EstablishmentId).IsRequired();

            builder.Property(f => f.IntValue).IsRequired(false);
            builder.Property(f => f.DateTimeValue).IsRequired(false);
            builder.Property(f => f.DecimalValue).IsRequired(false);
            builder.Property(f => f.StringValue).IsRequired(false);

            builder.HasOne(f => f.Facet).WithMany().HasForeignKey(f => f.FacetId);
            builder.HasOne(f => f.Environment).WithMany().HasForeignKey(f => f.EnvironmentId);
            builder.HasOne(f => f.Establishment).WithMany().HasForeignKey(f => new { f.EnvironmentId, f.EstablishmentId });
        }
    }
}
