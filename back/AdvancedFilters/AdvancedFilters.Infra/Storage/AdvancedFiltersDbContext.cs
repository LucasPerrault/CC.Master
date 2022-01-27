using AdvancedFilters.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace AdvancedFilters.Infra.Storage
{
    public class AdvancedFiltersDbContext : CloudControlDbContext<AdvancedFiltersDbContext>
    {
        public AdvancedFiltersDbContext(DbContextOptions<AdvancedFiltersDbContext> options)
            : base(options, StorageSchemas.AdvanceFilters)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CountriesConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentsConfiguration());
            modelBuilder.ApplyConfiguration(new AppInstancesConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentAccessesConfiguration());

            modelBuilder.ApplyConfiguration(new ClientsConfiguration());
            modelBuilder.ApplyConfiguration(new DistributorsConfiguration());
            modelBuilder.ApplyConfiguration(new LegalUnitsConfiguration());
            modelBuilder.ApplyConfiguration(new EstablishmentsConfiguration());
            modelBuilder.ApplyConfiguration(new EstablishmentContractsConfiguration());
            modelBuilder.ApplyConfiguration(new ContractsConfiguration());

            modelBuilder.ApplyConfiguration(new AppContactsConfiguration());
            modelBuilder.ApplyConfiguration(new ClientContactsConfiguration());
            modelBuilder.ApplyConfiguration(new SpecializedContactsConfiguration());

            modelBuilder.ApplyConfiguration(new FacetsConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentFacetValueDaosConfiguration());
            modelBuilder.ApplyConfiguration(new EstablishmentFacetValueDaosConfiguration());
        }

        public class AdvancedFiltersMigrationDefinition : CloudControlDbContextMigrationDefinition<AdvancedFiltersDbContext>
        {
            public override string SchemaName => StorageSchemas.AdvanceFilters.Value;
            public override int Order => 2;
        }
    }
}
