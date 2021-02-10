using Environments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Environments.Infra.Storage.Configurations
{
	public class EnvironmentsConfiguration : IEntityTypeConfiguration<Environment>
	{
		public void Configure(EntityTypeBuilder<Environment> builder)
		{
			builder.ToView("Environments");
			builder.HasKey(d => d.Id);
			builder.Property(d => d.Subdomain).HasColumnName("Subdomain");
			builder.Property(d => d.Domain).HasColumnName("Domain");
			builder.Property(d => d.Purpose).HasColumnName("Purpose");
			builder.Property(d => d.IsActive).HasColumnName("IsActive");

			builder.HasMany(d => d.ActiveAccesses).WithOne().HasForeignKey(d => d.EnvironmentId);

			builder.Ignore(d => d.ProductionHost);
		}
	}
}
