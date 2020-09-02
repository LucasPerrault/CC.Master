using Lucca.Core.AspNetCore.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Web
{
	public static class StorageConfigurer
	{
		public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<ConnectionStringOptions>(configuration.GetSection("SqlInfos"));
		}
	}
}
