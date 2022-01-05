using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpFilter.Infra.Migrations
{
    public partial class CreateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateTable(
                name: "IpFilterAuthorizationRequests",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpFilterAuthorizationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IpFilterAuthorizations",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpFilterAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpFilterAuthorizations_IpFilterAuthorizationRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "shared",
                        principalTable: "IpFilterAuthorizationRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IpFilterAuthorizations_RequestId",
                schema: "shared",
                table: "IpFilterAuthorizations",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IpFilterAuthorizations",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "IpFilterAuthorizationRequests",
                schema: "shared");
        }
    }
}
