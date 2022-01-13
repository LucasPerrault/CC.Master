using Billing.Contracts.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.ExternalId).HasColumnName("ExternalId");
            builder.Property(d => d.ClientId).HasColumnName("ClientId");
            builder.Property(d => d.ClientExternalId).HasColumnName("ClientExternalId");
            builder.Property(d => d.DistributorId).HasColumnName("DistributorId");
            builder.Property(d => d.CreatedAt).HasColumnName("CreatedAt");

            builder.Property(d => d.TheoreticalStartOn).HasColumnName("TheoreticalStartOn");
            builder.Property(d => d.TheoreticalEndOn).HasColumnName("TheoreticalEndOn");
            builder.Property(d => d.EndReason).HasColumnName("EndReason");

            builder.Property(d => d.ArchivedAt).HasColumnName("ArchivedAt");
            builder.Property(d => d.CommercialOfferId).HasColumnName("CommercialOfferId");

            builder.HasOne(d => d.Distributor).WithMany().HasForeignKey(d => d.DistributorId);
            builder.HasOne(d => d.DistributorBillingPreference).WithMany().HasForeignKey(d => d.DistributorId);
            builder.HasOne(d => d.Client).WithMany(c => c.Contracts).HasForeignKey(d => d.ClientId);
            builder.HasOne(d => d.CommercialOffer).WithMany().HasForeignKey(d => d.CommercialOfferId);

            builder.Property(d => d.EnvironmentId).HasColumnName("EnvironmentId");
            builder.HasOne(d => d.Environment).WithMany().HasForeignKey(d => d.EnvironmentId);


            builder.Property(d => d.CountEstimation).HasColumnName("CountEstimation");
            builder.Property(d => d.TheoreticalFreeMonths).HasColumnName("TheoreticalFreeMonths");
            builder.Property(d => d.RebatePercentage).HasColumnName("RebatePercentage");
            builder.Property(d => d.RebateEndsOn).HasColumnName("RebateEndsOn");
            builder.Property(d => d.MinimalBillingPercentage).HasColumnName("MinimalBillingPercentage");
            builder.Property(d => d.BillingPeriodicity).HasColumnName("BillingPeriodicity");

            builder.HasMany(d => d.Attachments).WithOne(d => d.Contract).HasForeignKey(a => a.ContractId);
        }
    }
}
