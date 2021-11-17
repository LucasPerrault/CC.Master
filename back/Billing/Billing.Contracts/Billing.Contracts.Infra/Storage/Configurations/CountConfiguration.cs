using Billing.Contracts.Domain.Counts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class CountConfiguration : IEntityTypeConfiguration<Count>
    {
        public void Configure(EntityTypeBuilder<Count> builder)
        {
            builder.ToTable("Counts");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ContractId).HasColumnName("ContractId");
            builder.Property(c => c.CommercialOfferId).HasColumnName("CommercialOfferId");
        }
    }
}
