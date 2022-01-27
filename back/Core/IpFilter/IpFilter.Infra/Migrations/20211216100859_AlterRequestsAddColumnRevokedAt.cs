using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpFilter.Infra.Migrations
{
    public partial class AlterRequestsAddColumnRevokedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                schema: "shared",
                table: "IpFilterAuthorizationRequests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedAt",
                schema: "shared",
                table: "IpFilterAuthorizationRequests");
        }
    }
}
