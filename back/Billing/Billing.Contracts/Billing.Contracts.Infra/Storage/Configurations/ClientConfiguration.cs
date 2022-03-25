using Billing.Contracts.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.ExternalId).HasColumnName("ExternalId");
            builder.Property(d => d.Name).HasColumnName("Name");
            builder.Property(d => d.SocialReason).HasColumnName("SocialReason");
            builder.Property(d => d.SalesforceId).HasColumnName("SalesforceId");
            builder.Property(d => d.BillingStreet).HasColumnName("BillingStreet");
            builder.Property(d => d.BillingPostalCode).HasColumnName("BillingPostalCode");
            builder.Property(d => d.BillingCity).HasColumnName("BillingCity");
            builder.Property(d => d.BillingCountry).HasColumnName("BillingCountry");
            builder.Property(d => d.BillingMail).HasColumnName("BillingMail");
            builder.Property(d => d.Phone).HasColumnName("Phone");
            builder.Property(d => d.BillingEntity).HasColumnName("BillingEntity");
        }
    }
}
