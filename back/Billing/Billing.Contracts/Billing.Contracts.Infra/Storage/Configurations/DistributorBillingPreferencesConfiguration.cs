using Billing.Contracts.Domain.Distributors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations;

public class DistributorBillingPreferencesConfiguration : IEntityTypeConfiguration<DistributorBillingPreference>
{
    public void Configure(EntityTypeBuilder<DistributorBillingPreference> builder)
    {
        builder.ToTable("DistributorBillingPreferences");
        builder.HasKey(p => p.DistributorId);
        builder.Property(p => p.IsEnforcingMinimalBilling).HasColumnName("IsEnforcingMinimalBilling");

        builder.HasOne(p => p.Distributor).WithOne().HasForeignKey<DistributorBillingPreference>(p => p.DistributorId);
    }
}
