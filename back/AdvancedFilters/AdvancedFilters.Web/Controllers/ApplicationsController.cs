using AdvancedFilters.Domain.Core.Collections;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;
using LuccaApplication = AdvancedFilters.Domain.Core.Models.Application;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/applications")]
    public class ApplicationsController
    {
        private readonly IApplicationsCollection _collection;

        public ApplicationsController(IApplicationsCollection collection)
        {
            _collection = collection;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<LuccaApplication>> GetApplicationsAsync(string search)
        {
            var apps = await _collection.GetAsync(search);
            return new Page<LuccaApplication>
            {
                Count = apps.Count,
                Items = apps
            };
        }
    }
}
