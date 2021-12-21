﻿// <auto-generated />
using System;
using AdvancedFilters.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AdvancedFilters.Infra.Migrations
{
    [DbContext(typeof(AdvancedFiltersDbContext))]
    partial class AdvancedFiltersDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("cafe")
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("BillingEntity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1)
                        .HasColumnName("BillingEntity");

                    b.Property<Guid>("ExternalId")
                        .HasMaxLength(36)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ExternalId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Clients", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Contract", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int")
                        .HasColumnName("EnvironmentId");

                    b.Property<Guid>("ExternalId")
                        .HasMaxLength(36)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ExternalId");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("Contracts", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Distributor", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<bool>("IsAllowingCommercialCommunication")
                        .HasColumnType("bit")
                        .HasColumnName("IsAllowingCommercialCommunication");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId")
                        .IsUnique();

                    b.ToTable("Distributors", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.EstablishmentContract", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int");

                    b.Property<int>("ContractId")
                        .HasColumnType("int");

                    b.HasKey("EnvironmentId", "EstablishmentId", "ContractId");

                    b.HasIndex("ContractId");

                    b.ToTable("EstablishmentContracts", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Contacts.Models.AppContact", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("AppInstanceId")
                        .HasColumnType("int")
                        .HasColumnName("AppInstanceId");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int")
                        .HasColumnName("EstablishmentId");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiresAt");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit")
                        .HasColumnName("IsConfirmed");

                    b.Property<string>("UserFirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserFirstName");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.Property<string>("UserLastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserLastName");

                    b.Property<string>("UserMail")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserMail");

                    b.HasKey("EnvironmentId", "Id");

                    b.HasIndex("EnvironmentId", "AppInstanceId");

                    b.HasIndex("EnvironmentId", "EstablishmentId");

                    b.ToTable("AppContacts", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Contacts.Models.ClientContact", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ClientId");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int")
                        .HasColumnName("EstablishmentId");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiresAt");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit")
                        .HasColumnName("IsConfirmed");

                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("RoleCode");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleId");

                    b.Property<string>("UserFirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserFirstName");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.Property<string>("UserLastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserLastName");

                    b.Property<string>("UserMail")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserMail");

                    b.HasKey("EnvironmentId", "Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("RoleCode");

                    b.HasIndex("EnvironmentId", "EstablishmentId");

                    b.ToTable("ClientContacts", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Contacts.Models.SpecializedContact", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<int>("EstablishmentId")
                        .HasColumnType("int")
                        .HasColumnName("EstablishmentId");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiresAt");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit")
                        .HasColumnName("IsConfirmed");

                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("RoleCode");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleId");

                    b.Property<string>("UserFirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserFirstName");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.Property<string>("UserLastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserLastName");

                    b.Property<string>("UserMail")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserMail");

                    b.HasKey("EnvironmentId", "Id");

                    b.HasIndex("RoleCode");

                    b.HasIndex("EnvironmentId", "EstablishmentId");

                    b.ToTable("SpecializedContacts", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Core.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.AppInstance", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ApplicationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ApplicationId");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("DeletedAt");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("EnvironmentId", "Id");

                    b.ToTable("AppInstances", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.Environment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Cluster")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Cluster");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Domain");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<string>("ProductionHost")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ProductionHost");

                    b.Property<string>("Subdomain")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)")
                        .HasColumnName("Subdomain");

                    b.HasKey("Id");

                    b.HasIndex("Subdomain");

                    b.ToTable("Environments", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.EnvironmentAccess", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int")
                        .HasColumnName("DistributorId");

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int")
                        .HasColumnName("EnvironmentId");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("Type");

                    b.HasKey("Id");

                    b.HasIndex("DistributorId");

                    b.HasIndex("EnvironmentId");

                    b.ToTable("EnvironmentAccesses", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.Establishment", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ActivityCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ActivityCode");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit")
                        .HasColumnName("IsArchived");

                    b.Property<string>("LegalIdentificationNumber")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("LegalIdentificationNumber");

                    b.Property<int>("LegalUnitId")
                        .HasColumnType("int")
                        .HasColumnName("LegalUnitId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<string>("TimeZoneId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("TimeZoneId");

                    b.Property<int>("UsersCount")
                        .HasColumnType("int")
                        .HasColumnName("UsersCount");

                    b.HasKey("EnvironmentId", "Id");

                    b.HasIndex("EnvironmentId", "LegalUnitId");

                    b.ToTable("Establishments", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.LegalUnit", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ActivityCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ActivityCode");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Code");

                    b.Property<int>("CountryId")
                        .HasColumnType("int")
                        .HasColumnName("CountryId");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<int?>("HeadquartersId")
                        .HasColumnType("int")
                        .HasColumnName("HeadquartersId");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit")
                        .HasColumnName("IsArchived");

                    b.Property<string>("LegalIdentificationNumber")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("LegalIdentificationNumber");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("EnvironmentId", "Id");

                    b.HasIndex("CountryId");

                    b.ToTable("LegalUnits", "cafe");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Contract", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Billing.Models.Client", "Client")
                        .WithMany("Contracts")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany("Contracts")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Environment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.EstablishmentContract", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Billing.Models.Contract", "Contract")
                        .WithMany("EstablishmentAttachments")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Establishment", "Establishment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId", "EstablishmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Contract");

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Contacts.Models.AppContact", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.AppInstance", "AppInstance")
                        .WithMany()
                        .HasForeignKey("EnvironmentId", "AppInstanceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Establishment", "Establishment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId", "EstablishmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AppInstance");

                    b.Navigation("Environment");

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Contacts.Models.ClientContact", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Billing.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .HasPrincipalKey("ExternalId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Establishment", "Establishment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId", "EstablishmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Environment");

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Contacts.Models.SpecializedContact", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Establishment", "Establishment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId", "EstablishmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Environment");

                    b.Navigation("Establishment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.AppInstance", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany("AppInstances")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Environment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.EnvironmentAccess", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Billing.Models.Distributor", "Distributor")
                        .WithMany("EnvironmentAccesses")
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany("Accesses")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Distributor");

                    b.Navigation("Environment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.Establishment", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany()
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.LegalUnit", "LegalUnit")
                        .WithMany("Establishments")
                        .HasForeignKey("EnvironmentId", "LegalUnitId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Environment");

                    b.Navigation("LegalUnit");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.LegalUnit", b =>
                {
                    b.HasOne("AdvancedFilters.Domain.Core.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AdvancedFilters.Domain.Instance.Models.Environment", "Environment")
                        .WithMany("LegalUnits")
                        .HasForeignKey("EnvironmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Country");

                    b.Navigation("Environment");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Client", b =>
                {
                    b.Navigation("Contracts");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Contract", b =>
                {
                    b.Navigation("EstablishmentAttachments");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Billing.Models.Distributor", b =>
                {
                    b.Navigation("EnvironmentAccesses");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.Environment", b =>
                {
                    b.Navigation("Accesses");

                    b.Navigation("AppInstances");

                    b.Navigation("Contracts");

                    b.Navigation("LegalUnits");
                });

            modelBuilder.Entity("AdvancedFilters.Domain.Instance.Models.LegalUnit", b =>
                {
                    b.Navigation("Establishments");
                });
#pragma warning restore 612, 618
        }
    }
}
