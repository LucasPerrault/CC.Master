using Lucca.Core.AspNetCore.EfMigration;
using Lucca.Core.AspNetCore.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Infra.Migrations
{
	public class CloudControlMigrationStartup : MigrationStartup
	{
		public const string MigrationsHistoryTableName = "__EFCoreMigrationsHistory";

		public override void ConfigureMigration(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<ConnectionStringOptions>(configuration.GetSection("SqlInfos"));
		}

		public override DatabaseMode DatabaseMode => DatabaseMode.MultiTenant;
	}
}
