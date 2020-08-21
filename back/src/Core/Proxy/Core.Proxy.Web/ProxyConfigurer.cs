using Microsoft.Extensions.DependencyInjection;
using ProxyKit;

namespace Core.Proxy.Web
{
	public class ProxyConfigurer
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddProxy();
		}
	}
}
