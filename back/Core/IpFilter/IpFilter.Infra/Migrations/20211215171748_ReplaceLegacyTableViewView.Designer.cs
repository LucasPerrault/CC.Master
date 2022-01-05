﻿// <auto-generated />
using System;
using IpFilter.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IpFilter.Infra.Migrations
{
    [DbContext(typeof(IpFilterDbContext))]
    [Migration("20211215171748_ReplaceLegacyTableViewView")]
    partial class ReplaceLegacyTableViewView
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("shared")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("IpFilter.Domain.IpFilterAuthorization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("Device")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Device");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiresAt");

                    b.Property<string>("IpAddress")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("IpAddress");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int")
                        .HasColumnName("RequestId");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("IpFilterAuthorizations", "shared");
                });

            modelBuilder.Entity("IpFilter.Domain.IpFilterAuthorizationRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Address");

                    b.Property<Guid>("Code")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Code");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiresAt");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.ToTable("IpFilterAuthorizationRequests", "shared");
                });

            modelBuilder.Entity("IpFilter.Domain.IpFilterAuthorization", b =>
                {
                    b.HasOne("IpFilter.Domain.IpFilterAuthorizationRequest", "Request")
                        .WithMany()
                        .HasForeignKey("RequestId");

                    b.Navigation("Request");
                });
#pragma warning restore 612, 618
        }
    }
}
