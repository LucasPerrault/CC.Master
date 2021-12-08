using Billing.Products.Domain.Interfaces;
using Billing.Products.Infra.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Products.Web
{
    public static class ProductsConfigurer
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProductsStore, ProductsStore>();
            services.AddScoped<IBusinessUnitsStore, BusinessUnitsStore>();
            services.AddScoped<ISolutionsStore, SolutionsStore>();
        }
    }
}
