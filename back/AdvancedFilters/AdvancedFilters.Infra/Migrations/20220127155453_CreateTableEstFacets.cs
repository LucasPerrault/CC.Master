using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedFilters.Infra.Migrations
{
    public partial class CreateTableEstFacets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstablishmentFacetValues",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacetId = table.Column<int>(type: "int", nullable: false),
                    EnvironmentId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    IntValue = table.Column<int>(type: "int", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentFacetValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstablishmentFacetValues_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentFacetValues_Establishments_EnvironmentId_EstablishmentId",
                        columns: x => new { x.EnvironmentId, x.EstablishmentId },
                        principalSchema: "cafe",
                        principalTable: "Establishments",
                        principalColumns: new[] { "EnvironmentId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentFacetValues_Facets_FacetId",
                        column: x => x.FacetId,
                        principalSchema: "cafe",
                        principalTable: "Facets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentFacetValues_EnvironmentId_EstablishmentId",
                schema: "cafe",
                table: "EstablishmentFacetValues",
                columns: new[] { "EnvironmentId", "EstablishmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentFacetValues_FacetId",
                schema: "cafe",
                table: "EstablishmentFacetValues",
                column: "FacetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablishmentFacetValues",
                schema: "cafe");
        }
    }
}
