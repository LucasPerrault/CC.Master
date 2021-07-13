﻿// <auto-generated />
using System;
using Billing.Products.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Billing.Products.Infra.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    partial class ProductDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("billing")
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Billing.Products.Domain.BusinessUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BusinessUnit");
                });

            modelBuilder.Entity("Billing.Products.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationCode")
                        .HasColumnName("ApplicationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnName("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeployRoute")
                        .HasColumnName("DeployRoute")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FamilyId")
                        .HasColumnName("FamilyId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEligibleToMinimalBilling")
                        .HasColumnName("IsEligibleToMinimalBilling")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFreeUse")
                        .HasColumnName("IsFreeUse")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMultiSuite")
                        .HasColumnName("IsMultiSuite")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPromoted")
                        .HasColumnName("IsPromoted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnName("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("SalesForceCode")
                        .HasColumnName("SalesForceCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Billing.Products.Domain.ProductFamily", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DisplayOrder")
                        .HasColumnName("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductFamilies");
                });

            modelBuilder.Entity("Billing.Products.Domain.ProductSolution", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("SolutionId")
                        .HasColumnType("int");

                    b.Property<int>("Share")
                        .HasColumnName("share")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "SolutionId");

                    b.HasIndex("SolutionId");

                    b.ToTable("ProductsSolutions");
                });

            modelBuilder.Entity("Billing.Products.Domain.Solution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BusinessUnitId")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnName("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DefaultBreakdownShare")
                        .HasColumnName("defaultBreakdownShare")
                        .HasColumnType("int");

                    b.Property<bool>("IsContactNeeded")
                        .HasColumnName("IsContactNeeded")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ParentId")
                        .HasColumnName("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BusinessUnitId");

                    b.ToTable("Solutions");
                });

            modelBuilder.Entity("Billing.Products.Domain.Product", b =>
                {
                    b.HasOne("Billing.Products.Domain.ProductFamily", "Family")
                        .WithMany()
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Billing.Products.Domain.ProductSolution", b =>
                {
                    b.HasOne("Billing.Products.Domain.Product", "Product")
                        .WithMany("ProductSolutions")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Billing.Products.Domain.Solution", "Solution")
                        .WithMany("ProductSolutions")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Billing.Products.Domain.Solution", b =>
                {
                    b.HasOne("Billing.Products.Domain.BusinessUnit", "BusinessUnit")
                        .WithMany("Solutions")
                        .HasForeignKey("BusinessUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
