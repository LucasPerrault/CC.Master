using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Environments.Infra.Migrations
{
    public partial class EnvironemntRenaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnvironmentRenaming",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: true),
                    apiKeyId = table.Column<int>(type: "int", nullable: true),
                    renamedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    oldName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    newName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    environmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentRenaming", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentRenaming_Environments_environmentId",
                        column: x => x.environmentId,
                        principalSchema: "dbo",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentRenaming_environmentId",
                schema: "shared",
                table: "EnvironmentRenaming",
                column: "environmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentRenaming",
                schema: "shared");
        }
    }
}
