using AdvancedFilters.Domain.Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class AppContactsConfiguration : IEntityTypeConfiguration<AppContact>
    {
        public void Configure(EntityTypeBuilder<AppContact> builder)
        {
            builder.ToTable("AppContacts");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RemoteId).HasColumnName("RemoteId").IsRequired();
            builder.Property(c => c.AppInstanceId).HasColumnName("AppInstanceId").IsRequired();
            builder.Property(c => c.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(c => c.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(c => c.ExpiresAt).HasColumnName("ExpiresAt");
            builder.Property(c => c.IsConfirmed).HasColumnName("IsConfirmed").IsRequired();

            builder.HasOne(c => c.AppInstance).WithMany().HasPrincipalKey(ai => ai.RemoteId).HasForeignKey(c => c.AppInstanceId);
        }
    }
}
