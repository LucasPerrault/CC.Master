using Microsoft.EntityFrameworkCore;
using System;

namespace Tools.Specflow
{
    public static class StartupContextHelper
    {
        private static string GetNameUniqueForTestContext(string name)
        {
            return $"{name}-{Guid.NewGuid()}";
        }

        public static TDbContext InitialiseDb<TDbContext>(Func<DbContextOptions<TDbContext>, TDbContext> newDbContext)
            where TDbContext : DbContext
        {
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(GetNameUniqueForTestContext("Instances"))
                .Options;

            var context = newDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
