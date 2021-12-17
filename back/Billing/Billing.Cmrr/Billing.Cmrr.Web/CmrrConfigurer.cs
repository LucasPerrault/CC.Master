using Billing.Cmrr.Application;
using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using Billing.Cmrr.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;
using Resources.Translations;

namespace Billing.Cmrr.Web
{
    public static class CmrrConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITranslations, Translations>();
            services.AddSingleton(new BreakDownInMemoryCache());

            services.AddScoped<ICmrrContractsStore, CmrrContractsStore>();
            services.AddScoped<ICmrrCountsStore, CmrrCountsStore>();
            services.AddScoped<IBreakdownService, BreakdownService>();
            services.AddScoped<ICmrrRightsFilter, CmrrRightsFilter>();

            services.AddScoped<ICmrrSituationsService, CmrrSituationsService>();
            services.AddScoped<ICmrrEvolutionsService, CmrrEvolutionsService>();
            services.AddScoped<IContractAxisSectionSituationsService, ContractAxisSectionSituationsService>();
        }
    }
}
