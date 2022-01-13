using Billing.Contracts.Domain.Counts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class CountConfiguration : IEntityTypeConfiguration<Count>
    {
        public void Configure(EntityTypeBuilder<Count> builder)
        {
            builder.ToTable("Counts");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CountPeriod).HasConversion<DateTime>(p => p, p => p).HasColumnName("CountPeriod");
            builder.Property(c => c.ContractId).HasColumnName("ContractId");
            builder.Property(c => c.CountPeriod).HasConversion<DateTime>(p => p, p => p).HasColumnName("CountPeriod");
            builder.Property(c => c.CommercialOfferId).HasColumnName("CommercialOfferId");
            builder.Property(c => c.FixedPrice).HasColumnName("FixedPrice");
            builder.Property(c => c.UnitPrice).HasColumnName("UnitPrice");
            builder.Property(c => c.Number).HasColumnName("Number");
            builder.Property(c => c.BillingStrategy).HasColumnName("BillingStrategy");

            builder.Ignore(c => c.TotalInCurrency);

            builder.HasOne(c => c.Contract).WithMany().HasForeignKey(c => c.ContractId);
        }
    }
}
