using AdvancedFilters.Domain.Billing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    class DistributorsConfiguration : IEntityTypeConfiguration<Distributor>
    {
        public void Configure(EntityTypeBuilder<Distributor> builder)
        {
            builder.ToTable("Distributors");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.Name).HasColumnName("Name").IsRequired();
            builder.Property(c => c.DepartmentId).HasColumnName("DepartmentId").IsRequired();
            builder.Property(c => c.IsLucca).HasColumnName("IsLucca").IsRequired().HasDefaultValue(false);
            builder.Property(c => c.IsAllowingCommercialCommunication).HasColumnName("IsAllowingCommercialCommunication").IsRequired();

            builder.HasIndex(e => e.DepartmentId).IsUnique();
        }
    }
}
