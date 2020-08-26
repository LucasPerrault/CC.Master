using Microsoft.Extensions.DependencyInjection;
using ProxyKit;

namespace Proxy.Web
{
	public class ProxyConfigurer
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddProxy();
		}
	}
}
