using System;

namespace Rights.Infra.Configuration
{
	public class RightsConfiguration
	{
		public Uri ServerUri { get; set; }
		public string DepartmentsEndpointPath { get; set; }
		public string ForeignAppEndpointPath { get; set; }
		public string UsersEndpointPath { get; set; }
	}
}
