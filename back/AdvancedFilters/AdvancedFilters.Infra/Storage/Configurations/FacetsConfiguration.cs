using AdvancedFilters.Domain.Facets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class FacetsConfiguration : IEntityTypeConfiguration<Facet>
    {
        public void Configure(EntityTypeBuilder<Facet> builder)
        {
            builder.ToTable("Facets");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.ApplicationId).IsRequired();
            builder.Property(f => f.Code).IsRequired();
            builder.Property(f => f.Type).IsRequired();
            builder.Property(f => f.Scope).IsRequired();

            builder.HasIndex(f => new { f.ApplicationId, f.Code }).IsUnique();
        }
    }
}
