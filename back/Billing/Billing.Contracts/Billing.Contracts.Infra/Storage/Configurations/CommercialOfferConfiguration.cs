using Billing.Contracts.Domain.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class CommercialOfferConfiguration : IEntityTypeConfiguration<CommercialOffer>
    {
        public void Configure(EntityTypeBuilder<CommercialOffer> builder)
        {
            builder.ToTable("CommercialOffers");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).HasColumnName("Name").IsRequired();
            builder.Property(d => d.Tag).HasColumnName("Tag");
            builder.Property(d => d.BillingMode).HasColumnName("BillingMode");
            builder.Property(d => d.Unit).HasColumnName("Unit");
            builder.Property(d => d.PricingMethod).HasColumnName("PricingMethod");
            builder.Property(d => d.ForecastMethod).HasColumnName("ForecastMethod");
            builder.Property(d => d.IsArchived).HasColumnName("IsArchived");
            builder.Property(d => d.CurrencyId).HasColumnName("CurrencyId");

            builder.Property(d => d.ProductId).HasColumnName("ProductId");
            builder.HasOne(d => d.Product).WithMany().HasForeignKey(d => d.ProductId);

            builder.HasMany(d => d.PriceLists).WithOne().HasForeignKey(d => d.OfferId);
        }
    }

    public class PriceListsConfiguration : IEntityTypeConfiguration<PriceList>
    {
        public void Configure(EntityTypeBuilder<PriceList> builder)
        {
            builder.ToTable("PriceLists");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.StartsOn).HasColumnName("StartsOn");

            builder.Property(d => d.OfferId).HasColumnName("OfferId");
            builder.HasMany(d => d.Rows).WithOne().HasForeignKey(d => d.ListId);
        }
    }

    public class PriceRowsConfiguration : IEntityTypeConfiguration<PriceRow>
    {
        public void Configure(EntityTypeBuilder<PriceRow> builder)
        {
            builder.ToTable("PriceRows");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FixedPrice).HasColumnName("FixedPrice");
            builder.Property(d => d.UnitPrice).HasColumnName("UnitPrice");
            builder.Property(d => d.MaxIncludedCount).HasColumnName("MaxIncludedCount");

            builder.Property(d => d.ListId).HasColumnName("ListId");
        }
    }

    public class ContractPricingsConfiguration : IEntityTypeConfiguration<ContractPricing>
    {
        public void Configure(EntityTypeBuilder<ContractPricing> builder)
        {
            builder.ToTable("ContractPricings");
            builder.HasNoKey();
            builder.Property(d => d.ContractId).HasColumnName("ContractId").IsRequired();
            builder.Property(d => d.PricingMethod).HasColumnName("PricingMethod");
            builder.Property(d => d.ConstantPrice).HasColumnName("ConstantPrice");
            builder.Property(d => d.AnnualCommitmentPrice).HasColumnName("AnnualCommitmentPrice");
            builder.Property(d => d.AnnualCommitmentUnits).HasColumnName("AnnualCommitmentUnits");
        }
    }
}
