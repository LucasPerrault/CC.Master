using Authentication.Domain;
using CloudControl.Shared.Infra.Remote.Services;
using Newtonsoft.Json;
using Partenaires.Infra.Configuration;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace Partenaires.Infra.Services
{
	public abstract class PartenairesService : HostRemoteService<PartenairesAuthServiceConfiguration>
	{
		protected const string _authScheme = "Lucca";

		protected override string RemoteAppName => "Partenaires";

		public PartenairesService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
			: base(httpClient, jsonSerializer)
		{
			Authenticate(claimsPrincipal);
		}

		private void Authenticate(ClaimsPrincipal claimsPrincipal)
		{
			var partenairesAuthConfig = new PartenairesAuthServiceConfiguration();

			string authType;
			Guid token;

			switch (claimsPrincipal)
			{
				case CloudControlUserClaimsPrincipal u:
					authType = "user";
					token = u.Token;
					break;
				case CloudControlApiKeyClaimsPrincipal ak:
					authType = "application";
					token = ak.Token;
					break;
				default:
					throw new ApplicationException("Can't get departments with unrecognized principal");
			}

			partenairesAuthConfig.Authenticate(_httpClient, _authScheme, authType, token);
		}
	}
}
