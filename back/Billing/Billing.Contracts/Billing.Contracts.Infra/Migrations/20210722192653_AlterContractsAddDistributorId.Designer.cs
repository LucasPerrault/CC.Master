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
    [Migration("20210722192653_AlterContractsAddDistributorId")]
    partial class AlterContractsAddDistributorId
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

                    b.HasIndex("DistributorId");

                    b.ToTable("Contracts");
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
                    b.HasOne("Distributors.Domain.Models.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
