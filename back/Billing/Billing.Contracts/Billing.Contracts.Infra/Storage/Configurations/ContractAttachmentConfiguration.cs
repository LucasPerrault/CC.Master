using Billing.Contracts.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class EstablishmentAttachmentConfiguration : IEntityTypeConfiguration<EstablishmentAttachment>
    {
        public void Configure(EntityTypeBuilder<EstablishmentAttachment> builder)
        {
            builder.ToTable("EstablishmentAttachments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.ContractId).HasColumnName("ContractId");
            builder.Property(d => d.EstablishmentId).HasColumnName("EstablishmentId");
            builder.Property(d => d.EstablishmentName).HasColumnName("EstablishmentName");
            builder.Property(d => d.EstablishmentRemoteId).HasColumnName("EstablishmentRemoteId");
            builder.Property(d => d.StartsOn).HasColumnName("StartsOn");
            builder.Property(d => d.EndsOn).HasColumnName("EndsOn");
            builder.Property(d => d.IsActive).HasColumnName("IsActive");
        }
    }
}
