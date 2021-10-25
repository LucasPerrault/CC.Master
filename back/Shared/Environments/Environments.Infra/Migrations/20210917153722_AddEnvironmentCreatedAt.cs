using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Environments.Infra.Migrations
{
    public partial class AddEnvironmentCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [shared].[Environments] WITH SCHEMABINDING AS
                    SELECT 
                    e.id id,
                    e.subdomain subdomain,
                    e.domain domain,
                    e.purpose purpose,
                    e.isActive isActive,
                    e.dtCreation CreatedAt
                FROM [dbo].[Environments] e
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
