﻿// <auto-generated />
using Distributors.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Distributors.Infra.Migrations
{
    [DbContext(typeof(DistributorsDbContext))]
    [Migration("20211223111423_AlterDistributorsAddIsLucca")]
    partial class AlterDistributorsAddIsLucca
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("shared")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Distributors.Domain.Models.Distributor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsAllowingCommercialCommunication")
                        .HasColumnType("bit")
                        .HasColumnName("IsAllowingCommercialCommunication");

                    b.Property<bool>("IsLucca")
                        .HasColumnType("bit")
                        .HasColumnName("IsLucca");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Distributors", "shared");
                });

            modelBuilder.Entity("Distributors.Domain.Models.DistributorDomain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("DistributorId");

                    b.Property<string>("Domain")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Domain");

                    b.HasKey("Id");

                    b.ToTable("DistributorDomains", "shared");
                });
#pragma warning restore 612, 618
        }
    }
}