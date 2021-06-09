﻿// <auto-generated />
using System;
using Environments.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Environments.Infra.Migrations
{
    [DbContext(typeof(EnvironmentsDbContext))]
    [Migration("20210601110147_RemoveToView")]
    partial class RemoveToView
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("shared")
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Distributors.Domain.Models.Distributor", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .HasColumnName("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Distributors","shared");
                });

            modelBuilder.Entity("Environments.Domain.Environment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Domain")
                        .HasColumnName("Domain")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnName("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Purpose")
                        .HasColumnName("Purpose")
                        .HasColumnType("int");

                    b.Property<string>("Subdomain")
                        .HasColumnName("Subdomain")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Environments","shared");
                });

            modelBuilder.Entity("Environments.Domain.EnvironmentAccess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId")
                        .HasColumnName("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnName("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DistributorId")
                        .HasColumnName("DistributorId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndsAt")
                        .HasColumnName("EndsAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("EnvironmentId")
                        .HasColumnName("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Lifecycle")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("int")
                        .HasComputedColumnSql("Lifecycle");

                    b.Property<string>("RevocationComment")
                        .HasColumnName("RevocationComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RevokedAt")
                        .HasColumnName("RevokedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnName("StartsAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnName("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("EnvironmentAccesses","shared");
                });

            modelBuilder.Entity("Environments.Domain.EnvironmentSharedAccess", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ConsumerId")
                        .HasColumnName("ConsumerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EnvironmentId")
                        .HasColumnName("EnvironmentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ConsumerId")
                        .IsUnique()
                        .HasFilter("[ConsumerId] IS NOT NULL");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("EnvironmentSharedAccesses","shared");
                });

            modelBuilder.Entity("Environments.Domain.EnvironmentSharedAccess", b =>
                {
                    b.HasOne("Distributors.Domain.Models.Distributor", "Consumer")
                        .WithOne()
                        .HasForeignKey("Environments.Domain.EnvironmentSharedAccess", "ConsumerId");

                    b.HasOne("Environments.Domain.Environment", null)
                        .WithMany("ActiveAccesses")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Environments.Domain.EnvironmentAccess", "Access")
                        .WithOne()
                        .HasForeignKey("Environments.Domain.EnvironmentSharedAccess", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
