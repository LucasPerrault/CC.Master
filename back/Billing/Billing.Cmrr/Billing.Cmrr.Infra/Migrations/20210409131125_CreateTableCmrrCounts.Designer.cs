﻿// <auto-generated />
using Billing.Cmrr.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Billing.Cmrr.Infra.Migrations
{
    [DbContext(typeof(CmrrDbContext))]
    [Migration("20210409131125_CreateTableCmrrCounts")]
    partial class CreateTableCmrrCounts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("billing")
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
#pragma warning restore 612, 618
        }
    }
}