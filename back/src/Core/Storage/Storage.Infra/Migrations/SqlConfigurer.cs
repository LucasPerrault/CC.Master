using Lucca.Core.AspNetCore.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Infra.Migrations
{
	public static class SqlConfigurer
	{
		public static void Configure(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<ConnectionStringOptions>(configuration.GetSection("SqlInfos"));
		}
	}
}
