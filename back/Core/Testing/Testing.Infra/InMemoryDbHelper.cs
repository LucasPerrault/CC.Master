using Microsoft.EntityFrameworkCore;
using System;

namespace Testing.Infra
{
    public static class InMemoryDbHelper
    {
        private static string GetNameUniqueForTestContext(string name)
        {
            return $"{name}-{Guid.NewGuid()}";
        }

        public static TDbContext InitialiseDb<TDbContext>(string name, Func<DbContextOptions<TDbContext>, TDbContext> newDbContext)
            where TDbContext : DbContext
        {
            try
            {
                var options = new DbContextOptionsBuilder<TDbContext>()
                    .UseInMemoryDatabase(GetNameUniqueForTestContext(name))
                    .Options;

                var context = newDbContext(options);
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                return context;
            }
            catch (Exception e)
            {
                throw new Exception($"{nameof(InMemoryDbHelper)} failed to initialise in memory db context", e);
            }
        }
    }
}
