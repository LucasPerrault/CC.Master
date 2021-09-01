using AdvancedFilters.Domain.Instance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class LegalUnitsConfiguration : IEntityTypeConfiguration<LegalUnit>
    {
        public void Configure(EntityTypeBuilder<LegalUnit> builder)
        {
            builder.ToTable("LegalUnits");
            builder.HasKey(lu => lu.Id);
            builder.Property(lu => lu.RemoteId).HasColumnName("RemoteId").IsRequired();
            builder.Property(lu => lu.EnvironmentId).HasColumnName("EnvironmentId").IsRequired();
            builder.Property(lu => lu.Name).HasColumnName("Name");
            builder.Property(lu => lu.Code).HasColumnName("Code");
            builder.Property(lu => lu.LegalIdentificationNumber).HasColumnName("LegalIdentificationNumber");
            builder.Property(lu => lu.ActivityCode).HasColumnName("ActivityCode");
            builder.Property(lu => lu.CountryId).HasColumnName("CountryId");
            builder.Property(lu => lu.HeadquartersId).HasColumnName("HeadquartersId");
            builder.Property(lu => lu.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(lu => lu.IsArchived).HasColumnName("IsArchived").IsRequired();

            builder.HasOne(lu => lu.Environment).WithMany().HasPrincipalKey(e => e.RemoteId).HasForeignKey(lu => lu.EnvironmentId);
            builder.HasMany(lu => lu.Establishments).WithOne().HasPrincipalKey(e => e.RemoteId).HasForeignKey(e => e.LegalUnitId);

            builder.HasIndex(e => e.RemoteId).IsUnique();
            builder.HasIndex(ai => ai.EnvironmentId);
        }
    }
}
