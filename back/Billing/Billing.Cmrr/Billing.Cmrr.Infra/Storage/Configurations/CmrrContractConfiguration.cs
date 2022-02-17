using Billing.Cmrr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Cmrr.Infra.Storage.Configurations
{
    public class CmrrContractConfiguration : IEntityTypeConfiguration<CmrrContract>
    {
        public void Configure(EntityTypeBuilder<CmrrContract> builder)
        {
            builder.ToView("CmrrContracts");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.ProductId).HasColumnName("ProductId");

            builder.Property(d => d.StartDate).HasColumnName("StartDate");
            builder.Property(d => d.EndDate).HasColumnName("EndDate");

            builder.Property(d => d.CreationCause).HasColumnName("CreationCause");
            builder.Property(d => d.EndReason).HasColumnName("EndReason");

            builder.Property(d => d.ClientId).HasColumnName("ClientId");
            builder.Property(d => d.ClientName).HasColumnName("ClientName");
            builder.Property(d => d.ClientBillingEntity).HasColumnName("ClientBillingEntity");

            builder.Property(d => d.DistributorId).HasColumnName("DistributorId");

            builder.Property(d => d.EnvironmentCreatedAt).HasColumnName("EnvironmentCreatedAt");

            builder.Property(d => d.IsArchived).HasColumnName("IsArchived");

            builder.HasOne(d => d.Distributor).WithMany().HasForeignKey(d => d.DistributorId);
        }
    }
}
