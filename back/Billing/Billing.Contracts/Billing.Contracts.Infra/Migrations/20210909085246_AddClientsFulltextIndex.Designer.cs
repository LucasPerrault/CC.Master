﻿// <auto-generated />
using System;
using Billing.Contracts.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Billing.Contracts.Infra.Migrations
{
    [DbContext(typeof(ContractsDbContext))]
    [Migration("20210909085246_AddClientsFulltextIndex")]
    partial class AddClientsFulltextIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("billing")
                .HasAnnotation("ProductVersion", "3.1.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Billing.Contracts.Domain.Clients.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BillingCity")
                        .HasColumnName("BillingCity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BillingCountry")
                        .HasColumnName("BillingCountry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BillingMail")
                        .HasColumnName("BillingMail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BillingPostalCode")
                        .HasColumnName("BillingPostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BillingState")
                        .HasColumnName("BillingState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BillingStreet")
                        .HasColumnName("BillingStreet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExternalId")
                        .HasColumnName("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnName("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SalesforceId")
                        .HasColumnName("SalesforceId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Contracts.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ArchivedAt")
                        .HasColumnName("ArchivedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ClientExternalId")
                        .HasColumnName("ClientExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ClientId")
                        .HasColumnName("ClientId")
                        .HasColumnType("int");

                    b.Property<int>("CommercialOfferId")
                        .HasColumnName("CommercialOfferId")
                        .HasColumnType("int");

                    b.Property<int>("DistributorId")
                        .HasColumnName("DistributorId")
                        .HasColumnType("int");

                    b.Property<int?>("EnvironmentId")
                        .HasColumnName("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<string>("EnvironmentSubdomain")
                        .HasColumnName("EnvironmentSubdomain")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExternalId")
                        .HasColumnName("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CommercialOfferId");

                    b.HasIndex("DistributorId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Contracts.EstablishmentAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContractId")
                        .HasColumnName("ContractId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndsOn")
                        .HasColumnName("EndsOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstablishmentId")
                        .HasColumnName("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<string>("EstablishmentName")
                        .IsRequired()
                        .HasColumnName("EstablishmentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EstablishmentRemoteId")
                        .HasColumnName("EstablishmentRemoteId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnName("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartsOn")
                        .HasColumnName("StartsOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ContractId");

                    b.ToTable("EstablishmentAttachments");
                });

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

            modelBuilder.Entity("Billing.Products.Domain.CommercialOffer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnName("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("CommercialOffers");
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
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductFamily");
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

                    b.Property<int?>("ParentId")
                        .HasColumnName("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BusinessUnitId");

                    b.ToTable("Solutions");
                });

            modelBuilder.Entity("Distributors.Domain.Models.Distributor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

            modelBuilder.Entity("Billing.Contracts.Domain.Contracts.Contract", b =>
                {
                    b.HasOne("Billing.Contracts.Domain.Clients.Client", "Client")
                        .WithMany("Contracts")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Billing.Products.Domain.CommercialOffer", "CommercialOffer")
                        .WithMany()
                        .HasForeignKey("CommercialOfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Distributors.Domain.Models.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Contracts.EstablishmentAttachment", b =>
                {
                    b.HasOne("Billing.Contracts.Domain.Contracts.Contract", null)
                        .WithMany("Attachments")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Billing.Products.Domain.CommercialOffer", b =>
                {
                    b.HasOne("Billing.Products.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
