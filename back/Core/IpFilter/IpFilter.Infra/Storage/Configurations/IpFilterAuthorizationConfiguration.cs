using IpFilter.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpFilter.Infra.Storage.Configurations
{
    public class IpFilterAuthorizationConfiguration: IEntityTypeConfiguration<IpFilterAuthorization>
    {
        public void Configure(EntityTypeBuilder<IpFilterAuthorization> builder)
        {
            builder.ToView("IpFilterAuthorizations");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Device).HasColumnName("Device");
            builder.Property(d => d.UserId).HasColumnName("UserID");
            builder.Property(d => d.IpAddress).HasColumnName("IpAddress");
            builder.Property(d => d.CreatedAt).HasColumnName("CreatedAt");
            builder.Property(d => d.ExpiresAt).HasColumnName("ExpiresAt");
        }
    }
}
