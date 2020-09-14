using Lucca.Core.AspNetCore.Tenancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Storage.Infra.Context
{
	public static class ContextConfigurer
	{
		public static void ConfigureContext<TContext>(this IServiceCollection services, IWebHostEnvironment environment)
			where TContext : CloudControlDbContext<TContext>
		{
			services.AddDbContextOnTenant<TContext>(DatabaseMode.MultiTenant, sqlOptions =>
			{
				sqlOptions
					.EnableRetryOnFailure(5);
			});
		}
	}
}
