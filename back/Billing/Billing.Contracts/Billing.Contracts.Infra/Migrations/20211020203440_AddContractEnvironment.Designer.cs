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
    [Migration("20211020203440_AddContractEnvironment")]
    partial class AddContractEnvironment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("billing")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Billing.Contracts.Domain.Clients.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BillingCity")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BillingCity");

                    b.Property<string>("BillingCountry")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BillingCountry");

                    b.Property<string>("BillingMail")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BillingMail");

                    b.Property<string>("BillingPostalCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BillingPostalCode");

                    b.Property<string>("BillingState")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BillingState");

                    b.Property<string>("BillingStreet")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BillingStreet");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ExternalId");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Phone");

                    b.Property<string>("SalesforceId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SalesforceId");

                    b.Property<string>("SocialReason")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SocialReason");

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
                        .HasColumnType("datetime2")
                        .HasColumnName("ArchivedAt");

                    b.Property<int>("BillingPeriodicity")
                        .HasColumnType("int")
                        .HasColumnName("BillingPeriodicity");

                    b.Property<Guid>("ClientExternalId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ClientExternalId");

                    b.Property<int>("ClientId")
                        .HasColumnType("int")
                        .HasColumnName("ClientId");

                    b.Property<int>("CommercialOfferId")
                        .HasColumnType("int")
                        .HasColumnName("CommercialOfferId");

                    b.Property<decimal>("CountEstimation")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("CountEstimation");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("DistributorId");

                    b.Property<int>("EndReason")
                        .HasColumnType("int")
                        .HasColumnName("EndReason");

                    b.Property<int?>("EnvironmentId")
                        .HasColumnType("int")
                        .HasColumnName("EnvironmentId");

                    b.Property<string>("EnvironmentSubdomain")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("EnvironmentSubdomain");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ExternalId");

                    b.Property<double>("MinimalBillingPercentage")
                        .HasColumnType("float")
                        .HasColumnName("MinimalBillingPercentage");

                    b.Property<DateTime?>("RebateEndsOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("RebateEndsOn");

                    b.Property<double?>("RebatePercentage")
                        .HasColumnType("float")
                        .HasColumnName("RebatePercentage");

                    b.Property<DateTime?>("TheoreticalEndOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("TheoreticalEndOn");

                    b.Property<int>("TheoreticalFreeMonths")
                        .HasColumnType("int")
                        .HasColumnName("TheoreticalFreeMonths");

                    b.Property<DateTime>("TheoreticalStartOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("TheoreticalStartOn");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CommercialOfferId");

                    b.HasIndex("DistributorId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Contracts.ContractComment", b =>
                {
                    b.Property<int>("ContractId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Comment");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("DistributorId");

                    b.HasKey("ContractId");

                    b.ToTable("ContractComments");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.ContractEnvironment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Subdomain")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Subdomain");

                    b.HasKey("Id");

                    b.ToTable("ContractEnvironments");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.Establishment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int")
                        .HasColumnName("EnvironmentId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<int?>("LegalUnitId")
                        .HasColumnType("int")
                        .HasColumnName("LegalUnitId");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int>("RemoteId")
                        .HasColumnType("int")
                        .HasColumnName("RemoteId");

                    b.HasKey("Id");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("Establishments");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.EstablishmentAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContractId")
                        .HasColumnType("int")
                        .HasColumnName("ContractId");

                    b.Property<DateTime?>("EndsOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("EndsOn");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int")
                        .HasColumnName("EstablishmentId");

                    b.Property<string>("EstablishmentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("EstablishmentName");

                    b.Property<int>("EstablishmentRemoteId")
                        .HasColumnType("int")
                        .HasColumnName("EstablishmentRemoteId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<DateTime>("StartsOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("StartsOn");

                    b.HasKey("Id");

                    b.HasIndex("ContractId");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("EstablishmentAttachments");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.EstablishmentExclusion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId")
                        .HasColumnType("int")
                        .HasColumnName("AuthorId");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int")
                        .HasColumnName("EstablishmentId");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("EstablishmentId");

                    b.ToTable("EstablishmentExclusions");
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
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductId");

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

            modelBuilder.Entity("Distributors.Domain.Models.Distributor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<bool>("IsAllowingCommercialCommunication")
                        .HasColumnType("bit")
                        .HasColumnName("IsAllowingCommercialCommunication");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Distributors", "shared");
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

                    b.Navigation("Client");

                    b.Navigation("CommercialOffer");

                    b.Navigation("Distributor");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.Establishment", b =>
                {
                    b.HasOne("Billing.Contracts.Domain.Environments.ContractEnvironment", null)
                        .WithMany("Establishments")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.EstablishmentAttachment", b =>
                {
                    b.HasOne("Billing.Contracts.Domain.Contracts.Contract", null)
                        .WithMany("Attachments")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Billing.Contracts.Domain.Environments.Establishment", null)
                        .WithMany("Attachments")
                        .HasForeignKey("EstablishmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.EstablishmentExclusion", b =>
                {
                    b.HasOne("Billing.Contracts.Domain.Environments.Establishment", null)
                        .WithMany("Exclusions")
                        .HasForeignKey("EstablishmentId")
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

                    b.Navigation("Product");
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

            modelBuilder.Entity("Billing.Contracts.Domain.Clients.Client", b =>
                {
                    b.Navigation("Contracts");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Contracts.Contract", b =>
                {
                    b.Navigation("Attachments");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.ContractEnvironment", b =>
                {
                    b.Navigation("Establishments");
                });

            modelBuilder.Entity("Billing.Contracts.Domain.Environments.Establishment", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Exclusions");
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