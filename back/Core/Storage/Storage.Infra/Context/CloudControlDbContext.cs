using Lucca.Core.Api.Queryable.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Storage.Infra.Context
{
    public abstract class CloudControlDbContext<T> : DbContext where T : DbContext
    {
        private readonly StorageSchema _storageSchema;

        protected CloudControlDbContext(DbContextOptions<T> options, StorageSchema storageSchema) : base(options)
        {
            _storageSchema = storageSchema;
        }

        private string Schema => _storageSchema.Value;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(ModelBuilder));
            }

            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.RegisterPagingMethods();

            ApplyConfiguration(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        protected abstract void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}