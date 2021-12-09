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
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
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
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ApplicationCode");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<string>("DeployRoute")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("DeployRoute");

                    b.Property<int>("FamilyId")
                        .HasColumnType("int")
                        .HasColumnName("FamilyId");

                    b.Property<bool>("IsEligibleToMinimalBilling")
                        .HasColumnType("bit")
                        .HasColumnName("IsEligibleToMinimalBilling");

                    b.Property<bool>("IsFreeUse")
                        .HasColumnType("bit")
                        .HasColumnName("IsFreeUse");

                    b.Property<bool>("IsMultiSuite")
                        .HasColumnType("bit")
                        .HasColumnName("IsMultiSuite");

                    b.Property<bool>("IsPromoted")
                        .HasColumnType("bit")
                        .HasColumnName("IsPromoted");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("ParentId");

                    b.Property<string>("SalesForceCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SalesForceCode");

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
                        .HasColumnType("int")
                        .HasColumnName("DisplayOrder");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

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
                        .HasColumnType("int")
                        .HasColumnName("share");

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
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<int>("DefaultBreakdownShare")
                        .HasColumnType("int")
                        .HasColumnName("defaultBreakdownShare");

                    b.Property<bool>("IsContactNeeded")
                        .HasColumnType("bit")
                        .HasColumnName("IsContactNeeded");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("ParentId");

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

                    b.Navigation("Family");
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

                    b.Navigation("Product");

                    b.Navigation("Solution");
                });

            modelBuilder.Entity("Billing.Products.Domain.Solution", b =>
                {
                    b.HasOne("Billing.Products.Domain.BusinessUnit", "BusinessUnit")
                        .WithMany("Solutions")
                        .HasForeignKey("BusinessUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BusinessUnit");
                });

            modelBuilder.Entity("Billing.Products.Domain.BusinessUnit", b =>
                {
                    b.Navigation("Solutions");
                });

            modelBuilder.Entity("Billing.Products.Domain.Product", b =>
                {
                    b.Navigation("ProductSolutions");
                });

            modelBuilder.Entity("Billing.Products.Domain.Solution", b =>
                {
                    b.Navigation("ProductSolutions");
                });
#pragma warning restore 612, 618
        }
    }
}
