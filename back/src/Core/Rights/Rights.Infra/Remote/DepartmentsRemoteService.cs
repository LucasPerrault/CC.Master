﻿using Newtonsoft.Json;
using Partenaires.Infra.Services;
using Rights.Infra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
	public class DepartmentsRemoteService : PartenairesService
	{
		public DepartmentsRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
			: base(httpClient, jsonSerializer, claimsPrincipal)
		{ }

		internal async Task<IReadOnlyCollection<Department>> GetDepartmentsAsync()
		{
			var queryParams = new Dictionary<string, string>();

			var departmentsResponse = await GetObjectCollectionResponseAsync<Department>(queryParams);

			return departmentsResponse.Data.Items.ToList();
		}
	}
}