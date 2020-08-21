using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CloudControl.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args).ConfigureServices(s =>
				{
					s.AddSingleton<ServicesConfiguration>();
				})
				.UseStartup<Startup>();
	}
}
