using Lucca.Core.AspNetCore.Tenancy.Extractors;
using Microsoft.Extensions.Primitives;

namespace CloudControl.Web.Tests.Mocks
{
	public class TestTenantExtractor : ITenantExtractor
	{
		public bool TryGetTenant(out StringSegment tenant)
		{
			tenant = "test";
			return true;
		}
	}
}
