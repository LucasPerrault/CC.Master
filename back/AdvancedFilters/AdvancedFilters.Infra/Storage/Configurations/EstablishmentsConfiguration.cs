using AdvancedFilters.Domain.Instance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class EstablishmentsConfiguration : IEntityTypeConfiguration<Establishment>
    {
        public void Configure(EntityTypeBuilder<Establishment> builder)
        {
            builder.ToTable("Establishments");
            builder.HasKey(e => new { e.EnvironmentId, e.Id });

            builder.Property(e => e.Name).HasColumnName("Name").IsRequired();
            builder.Property(e => e.Code).HasColumnName("Code");
            builder.Property(e => e.LegalUnitId).HasColumnName("LegalUnitId").IsRequired();
            builder.Property(e => e.LegalIdentificationNumber).HasColumnName("LegalIdentificationNumber");
            builder.Property(e => e.ActivityCode).HasColumnName("ActivityCode");
            builder.Property(e => e.TimeZoneId).HasColumnName("TimeZoneId").IsRequired();
            builder.Property(e => e.UsersCount).HasColumnName("UsersCount").IsRequired();
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(e => e.IsArchived).HasColumnName("IsArchived").IsRequired();

            builder
                .HasOne(e => e.Environment)
                .WithMany()
                .HasForeignKey(e => e.EnvironmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(e => e.LegalUnit)
                .WithMany(lu => lu.Establishments)
                .HasForeignKey(e => new { e.EnvironmentId, e.LegalUnitId })
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
