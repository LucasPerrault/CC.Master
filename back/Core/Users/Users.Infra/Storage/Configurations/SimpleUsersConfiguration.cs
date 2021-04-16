using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Infra.Storage.Configurations
{
    public class SimpleUsersConfiguration: IEntityTypeConfiguration<SimpleUser>
    {
        public void Configure(EntityTypeBuilder<SimpleUser> builder)
        {
            builder.ToTable("Users");
            builder.Property(u => u.Id).HasColumnName("PartenairesId").ValueGeneratedNever();
            builder.Property(u => u.FirstName).HasColumnName("FirstName").HasMaxLength(300);
            builder.Property(u => u.LastName).HasColumnName("LastName").HasMaxLength(300);
            builder.Property(u => u.DepartmentId).HasColumnName("DepartmentId");
            builder.Property(u => u.IsActive).HasColumnName("IsActive");

            builder.HasIndex(u => u.Id).IsUnique();
        }
    }
}
