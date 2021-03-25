﻿// <auto-generated />
using System;
using Instances.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Instances.Infra.Migrations
{
    [DbContext(typeof(InstancesDbContext))]
    partial class InstancesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("instances")
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Instances.Domain.Demos.Demo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .HasColumnName("comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DeletionScheduledOn")
                        .HasColumnName("deletionScheduledOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DistributorID")
                        .HasColumnName("distributorID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("InstanceID")
                        .HasColumnName("instanceId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnName("isActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTemplate")
                        .HasColumnType("bit");

                    b.Property<string>("Subdomain")
                        .HasColumnName("subdomain")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DistributorID");

                    b.HasIndex("InstanceID")
                        .IsUnique();

                    b.ToTable("Demos");
                });

            modelBuilder.Entity("Instances.Domain.Demos.DemoDuplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId")
                        .HasColumnName("authorId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnName("comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("createdAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DistributorId")
                        .HasColumnName("distributorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .HasColumnName("password")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("Progress")
                        .HasColumnName("progress")
                        .HasColumnType("int");

                    b.Property<string>("SourceDemoSubdomain")
                        .HasColumnName("sourceDemoSubdomain")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Subdomain")
                        .HasColumnName("subdomain")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("DistributorId");

                    b.ToTable("DemoDuplications");
                });

            modelBuilder.Entity("Instances.Domain.Demos.Demo", b =>
                {
                    b.HasOne("Distributors.Domain.Models.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorID");

                    b.HasOne("Instances.Domain.Instances.Models.Instance", "Instance")
                        .WithOne()
                        .HasForeignKey("Instances.Domain.Demos.Demo", "InstanceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Instances.Domain.Demos.DemoDuplication", b =>
                {
                    b.HasOne("Distributors.Domain.Models.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorId");
                });
#pragma warning restore 612, 618
        }
    }
}
