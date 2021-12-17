using IpFilter.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpFilter.Infra.Storage.Configurations
{
    public class IpFilterAuthorizationRequestConfiguration: IEntityTypeConfiguration<IpFilterAuthorizationRequest>
    {
        public void Configure(EntityTypeBuilder<IpFilterAuthorizationRequest> builder)
        {

            builder.ToTable("IpFilterAuthorizationRequests");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Address).HasColumnName("Address");
            builder.Property(d => d.UserId).HasColumnName("UserId");
            builder.Property(d => d.Code).HasColumnName("Code");
            builder.Property(d => d.CreatedAt).HasColumnName("CreatedAt");
            builder.Property(d => d.ExpiresAt).HasColumnName("ExpiresAt");
            builder.Property(d => d.RevokedAt).HasColumnName("RevokedAt");
        }
    }
}
