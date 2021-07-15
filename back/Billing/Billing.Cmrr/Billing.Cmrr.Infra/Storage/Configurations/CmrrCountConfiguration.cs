using Billing.Cmrr.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Cmrr.Infra.Storage.Configurations
{
    public class CmrrCountConfiguration : IEntityTypeConfiguration<CmrrCount>
    {
        public void Configure(EntityTypeBuilder<CmrrCount> builder)
        {
            builder.ToView("CmrrCounts");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.ContractId).HasColumnName("ContractId");
            builder.Property(d => d.CountPeriod).HasColumnName("CountPeriod");
            builder.Property(d => d.BillingStrategy).HasColumnName("BillingStrategy");
            builder.Property(d => d.AccountingNumber).HasColumnName("AccountingNumber");
            builder.Property(d => d.EuroTotal).HasColumnName("EuroTotal");
        }
    }
}
