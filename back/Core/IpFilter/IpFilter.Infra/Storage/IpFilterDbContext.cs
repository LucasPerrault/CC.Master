using IpFilter.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;

namespace IpFilter.Infra.Storage
{
    public class IpFilterDbContext: CloudControlDbContext<IpFilterDbContext>
    {
        public IpFilterDbContext(DbContextOptions<IpFilterDbContext> options)
            : base(options, StorageSchemas.Shared)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IpFilterAuthorizationConfiguration());
        }
    }
}
