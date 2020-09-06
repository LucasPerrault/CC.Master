using Distributors.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;

namespace Distributors.Infra.Storage
{
	public class DistributorsDbContext : CloudControlDbContext<DistributorsDbContext>
	{
		public DistributorsDbContext(DbContextOptions<DistributorsDbContext> options)
			: base(options, StorageSchemas.Shared)
		{ }

		protected override void ApplyConfiguration(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new DistributorsConfiguration());
		}
	}
}
