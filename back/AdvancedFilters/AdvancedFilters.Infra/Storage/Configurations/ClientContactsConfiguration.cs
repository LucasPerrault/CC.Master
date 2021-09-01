using AdvancedFilters.Domain.Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class ClientContactsConfiguration : IEntityTypeConfiguration<ClientContact>
    {
        public void Configure(EntityTypeBuilder<ClientContact> builder)
        {
            builder.ToTable("ClientContacts");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RemoteId).HasColumnName("RemoteId").IsRequired();
            builder.Property(c => c.RoleId).HasColumnName("RoleId").IsRequired();
            builder.Property(c => c.ClientId).HasColumnName("ClientId").IsRequired();
            builder.Property(c => c.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(c => c.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(c => c.ExpiresAt).HasColumnName("ExpiresAt");
            builder.Property(c => c.IsConfirmed).HasColumnName("IsConfirmed").IsRequired();
            builder.Property(c => c.EnvironmentId).HasColumnName("EnvironmentId").IsRequired();

            builder.HasOne(c => c.Client).WithMany().HasPrincipalKey(c => c.ExternalId).HasForeignKey(c => c.ClientId);
            builder.HasOne(c => c.Environment).WithMany().HasPrincipalKey(e => e.RemoteId).HasForeignKey(c => c.EnvironmentId);

            builder.HasIndex(ai => ai.RemoteId).IsUnique();
            builder.HasIndex(ai => ai.EnvironmentId);
            builder.HasIndex(ai => ai.ClientId);
            builder.HasIndex(ai => ai.RoleId);
        }
    }
}
