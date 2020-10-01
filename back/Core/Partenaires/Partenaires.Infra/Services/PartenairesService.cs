using Authentication.Domain;
using Newtonsoft.Json;
using Partenaires.Infra.Configuration;
using Remote.Infra.Services;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace Partenaires.Infra.Services
{
	public abstract class PartenairesService : RestApiV3HostRemoteService<PartenairesAuthServiceConfiguration>
	{
		protected const string _authScheme = "Lucca";
		protected override string RemoteAppName => "Partenaires";

		protected readonly ClaimsPrincipal _claimsPrincipal;

		public PartenairesService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
			: base(httpClient, jsonSerializer)
		{
			_claimsPrincipal = claimsPrincipal;
			Authenticate(_claimsPrincipal);
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
					throw new ApplicationException("Can't authenticate to Lucca service with unrecognized principal");
			}

			partenairesAuthConfig.Authenticate(_httpClient, _authScheme, authType, token);
		}
	}
}
