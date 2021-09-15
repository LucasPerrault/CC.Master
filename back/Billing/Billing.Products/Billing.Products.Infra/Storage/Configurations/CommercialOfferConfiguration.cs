using Billing.Products.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Products.Infra.Storage.Configurations
{
    public class CommercialOfferConfiguration : IEntityTypeConfiguration<CommercialOffer>
    {
        public void Configure(EntityTypeBuilder<CommercialOffer> builder)
        {
            builder.ToTable("CommercialOffers");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Name).HasColumnName("Name");
            builder.Property(o => o.ProductId).HasColumnName("ProductId");

            builder.HasOne(o => o.Product).WithMany().HasForeignKey(o => o.ProductId);
        }
    }
}
