using AdvancedFilters.Domain.Billing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class EstablishmentContractsConfiguration : IEntityTypeConfiguration<EstablishmentContract>
    {
        public void Configure(EntityTypeBuilder<EstablishmentContract> builder)
        {
            builder.ToTable("EstablishmentContracts");
            builder.HasKey(ec => new { ec.EnvironmentId, ec.EstablishmentId, ec.ContractId });

            builder.HasOne(ec => ec.Contract).WithMany().HasForeignKey(ec => ec.ContractId);
            builder.HasOne(ec => ec.Establishment).WithMany().HasForeignKey(ec => new { ec.EnvironmentId, ec.EstablishmentId });

            builder.HasIndex(ec => ec.ContractId);
        }
    }
}
