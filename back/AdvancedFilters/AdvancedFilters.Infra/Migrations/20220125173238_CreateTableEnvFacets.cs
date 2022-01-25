using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedFilters.Infra.Migrations
{
    public partial class CreateTableEnvFacets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facets",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentFacetValues",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacetId = table.Column<int>(type: "int", nullable: false),
                    EnvironmentId = table.Column<int>(type: "int", nullable: false),
                    IntValue = table.Column<int>(type: "int", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentFacetValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentFacetValues_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvironmentFacetValues_Facets_FacetId",
                        column: x => x.FacetId,
                        principalSchema: "cafe",
                        principalTable: "Facets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentFacetValues_EnvironmentId",
                schema: "cafe",
                table: "EnvironmentFacetValues",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentFacetValues_FacetId",
                schema: "cafe",
                table: "EnvironmentFacetValues",
                column: "FacetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentFacetValues",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Facets",
                schema: "cafe");
        }
    }
}
