using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Infra.Context;
using System;
using Xunit;

namespace Testing.Infra.Tests
{
    internal class TestDbContext : CloudControlDbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options, StorageSchemas.Core)
        { }

        internal class TestEntity
        {
            public int Id { get; set; }
            public int Name { get; set; }
        }

        internal class TestEntityDbConfig : IEntityTypeConfiguration<TestEntity>
        {
            public void Configure(EntityTypeBuilder<TestEntity> builder)
            {
                builder.ToTable("TestEntities");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Name).HasColumnName("Name").IsRequired();
            }
        }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TestEntityDbConfig());
        }
    }

    public class InMemoryDbHelperTests
    {
        [Fact]
        public void ShouldInitialiseInMemoryDb()
        {
            Func<TestDbContext> func = () => InMemoryDbHelper.InitialiseDb<TestDbContext>("hello", o => new TestDbContext(o));
            func.Should().NotThrow();
        }
    }
}
