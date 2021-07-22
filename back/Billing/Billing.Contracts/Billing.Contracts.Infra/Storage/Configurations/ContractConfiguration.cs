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
            builder.Property(d => d.EnvironmentId).HasColumnName("EnvironmentId");
            builder.Property(d => d.DistributorId).HasColumnName("DistributorId");
            builder.Property(d => d.EnvironmentSubdomain).HasColumnName("EnvironmentSubdomain");
            builder.Property(d => d.ArchivedAt).HasColumnName("ArchivedAt");

            builder.HasOne(d => d.Distributor).WithMany().HasForeignKey(d => d.DistributorId);
            builder.HasOne(d => d.Client).WithMany(c => c.Contracts).HasForeignKey(d => d.ClientId);
        }
    }
}
