using AdvancedFilters.Domain.Contacts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class SpecializedContactsConfiguration : IEntityTypeConfiguration<SpecializedContact>
    {
        public void Configure(EntityTypeBuilder<SpecializedContact> builder)
        {
            builder.ToTable("SpecializedContacts");
            builder.HasKey(c => new { c.EnvironmentId, c.Id });

            builder.Property(c => c.EstablishmentId).HasColumnName("EstablishmentId").IsRequired();
            builder.Property(c => c.RoleId).HasColumnName("RoleId").IsRequired();
            builder.Property(c => c.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(c => c.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(c => c.ExpiresAt).HasColumnName("ExpiresAt");
            builder.Property(c => c.IsConfirmed).HasColumnName("IsConfirmed").IsRequired();

            builder
                .HasOne(c => c.Environment)
                .WithMany()
                .HasForeignKey(c => c.EnvironmentId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(c => c.Establishment)
                .WithMany()
                .HasForeignKey(c => new { c.EnvironmentId, c.EstablishmentId })
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
