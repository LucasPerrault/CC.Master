using Environments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Environments.Infra.Storage.Configurations
{
    public class EnvironmentRenamingConfiguration : IEntityTypeConfiguration<EnvironmentRenaming>
    {
        public void Configure(EntityTypeBuilder<EnvironmentRenaming> builder)
        {
            builder.ToTable("EnvironmentRenaming");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId).HasColumnName("userId");
            builder.Property(r => r.ApiKeyStorageId).HasColumnName("apiKeyStorageId");
            builder.Property(r => r.RenamedOn).HasColumnName("renamedOn");
            builder.Property(r => r.OldName).HasColumnName("oldName");
            builder.Property(r => r.NewName).HasColumnName("newName");
            builder.Property(r => r.Status).HasColumnName("status");
            builder.Property(r => r.ErrorMessage).HasColumnName("errorMessage");

            builder.Property(r => r.EnvironmentId).HasColumnName("environmentId");
            builder.HasOne(r => r.Environment).WithMany().HasForeignKey(r => r.EnvironmentId);
        }
    }
}
