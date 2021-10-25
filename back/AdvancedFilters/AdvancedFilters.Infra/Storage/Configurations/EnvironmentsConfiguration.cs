using AdvancedFilters.Domain.Instance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class EnvironmentsConfiguration : IEntityTypeConfiguration<Environment>
    {
        public void Configure(EntityTypeBuilder<Environment> builder)
        {
            builder.ToTable("Environments");
            builder.HasKey(e => e.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(e => e.Subdomain).HasColumnName("Subdomain").IsRequired().HasMaxLength(63);
            builder.Property(e => e.Domain).HasColumnName("Domain").IsRequired();
            builder.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(e => e.ProductionHost).HasColumnName("ProductionHost").IsRequired();

            builder.HasIndex(e => e.Subdomain).IsClustered(false);
        }
    }
}
