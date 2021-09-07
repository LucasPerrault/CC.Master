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

            builder.Property(e => e.Subdomain).HasColumnName("Subdomain").IsRequired();
            builder.Property(e => e.Domain).HasColumnName("Domain").IsRequired();
            builder.Property(e => e.IsActive).HasColumnName("IsActive").IsRequired();

            builder.HasMany(e => e.LegalUnits).WithOne().HasForeignKey(lu => lu.EnvironmentId);
        }
    }
}
