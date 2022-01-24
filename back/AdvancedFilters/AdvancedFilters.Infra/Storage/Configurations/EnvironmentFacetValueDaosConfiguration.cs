using AdvancedFilters.Infra.Storage.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class EnvironmentFacetValueDaosConfiguration : IEntityTypeConfiguration<EnvironmentFacetValueDao>
    {
        public void Configure(EntityTypeBuilder<EnvironmentFacetValueDao> builder)
        {
            builder.ToTable("EnvironmentFacetValues");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.FacetId).IsRequired();
            builder.Property(f => f.EnvironmentId).IsRequired();

            builder.Property(f => f.IntValue).IsRequired(false);
            builder.Property(f => f.DateTimeValue).IsRequired(false);
            builder.Property(f => f.DecimalValue).IsRequired(false);
            builder.Property(f => f.StringValue).IsRequired(false);

            builder.HasOne(f => f.Facet).WithMany().HasForeignKey(f => f.FacetId);
            builder.HasOne(f => f.Environment).WithMany().HasForeignKey(f => f.EnvironmentId);
        }
    }
}
